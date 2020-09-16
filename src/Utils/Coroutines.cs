using System;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Collections.Generic;

namespace Classroom_Client
{
    #region Coroutine Functions
    public partial class Utils
    {
        public static object[] StartValuedCoroutine(IEnumerator Coroutine) => StartCoroutineInternal(Coroutine, (v) => { });
        public static void StartCoroutine(IEnumerator Coroutine) => new ThreadWrapper(() => StartCoroutineInternal(Coroutine, (v) => { }), Coroutine).Start();

        public static object[] StartValuedCoroutine(IEnumerator Coroutine, Action<object> action) => StartCoroutineInternal(Coroutine, action);
        public static void StartCoroutine(IEnumerator Coroutine, Action<object> action) => new ThreadWrapper(() => StartCoroutineInternal(Coroutine, action), Coroutine).Start();

        public static object[] StartValuedCoroutine(IEnumerator Coroutine, Action<object, int> action) => StartCoroutineInternal(Coroutine, null, action);
        public static void StartCoroutine(IEnumerator Coroutine, Action<object, int> action) => new ThreadWrapper(() => StartCoroutineInternal(Coroutine, null, action), Coroutine).Start();

        public static void StopAllCoroutines()
        {
            foreach (var pair in runningCoroutines)
            {
                foreach (var wrapper in pair.Value)
                {
                    wrapper.Stop();
                }
            }
            runningCoroutines.Clear();
        }
        public static void StopCoroutine(IEnumerator Coroutine)
        {
            if (!runningCoroutines.ContainsKey(Coroutine.ToString())) return;
            foreach (var wrapper in runningCoroutines[Coroutine.ToString()])
            {
                wrapper.Stop();
            }
            runningCoroutines.Remove(Coroutine.ToString());
        }

        private static Dictionary<string, List<ThreadWrapper>> runningCoroutines = new Dictionary<string, List<ThreadWrapper>>();

        private class ThreadWrapper
        {
            private Thread goThread;
            private IEnumerator Coroutine;

            public void Start() => goThread.Start();

            public void Stop()
            {
                try
                {
                    goThread.Abort();
                }
                catch (PlatformNotSupportedException)
                {
                    goThread.Interrupt();
                }
            }

            public ThreadWrapper(Action act, IEnumerator coroutine)
            {
                goThread = new Thread(() => { try { act(); } catch (Exception ex) { if (!(ex is ThreadInterruptedException || ex is ThreadAbortException)) { throw; } return; } });
                Coroutine = coroutine;
                if (runningCoroutines.ContainsKey(Coroutine.ToString())) runningCoroutines[Coroutine.ToString()].Add(this);
                else { runningCoroutines.Add(Coroutine.ToString(), new List<ThreadWrapper>() { this }); }
            }
        }

        private static object[] StartCoroutineInternal(IEnumerator Coroutine, Action<object> act, Action<object, int> act2 = null)
        {
            List<object> toReturn = new List<object>();
            Action<object, int> action = null;
            if (act == null)
            {
                action = (v, i) => act2(v, i);
            }
            else
            {
                action = (v, i) => act(v);
            }
            int index = 0;
            while (Coroutine.MoveNext())
            {
                if (Coroutine.Current == null)
                {
                    toReturn.Add(null);
                    action(null, index++);
                    continue;
                }
                try
                {
                    CustomYieldable yielded = (CustomYieldable)Coroutine.Current;
                    if (yielded.Yield)
                    {
                        toReturn.Add(Coroutine.Current);
                        action(toReturn[toReturn.Count - 1], index++);
                    }
                }
                catch (InvalidCastException) { }
                try
                {
                    toReturn.Add(StartValuedCoroutine((IEnumerator)Coroutine.Current, action));
                    action(toReturn[toReturn.Count - 1], index++);
                    continue;
                }
                catch (InvalidCastException) { }
                if (Coroutine.Current.GetType().GetMethod("GetAwaiter") != null)
                {
                    Type cType = Coroutine.Current.GetType();
                    MethodInfo awaitMethod = cType.GetMethod("GetAwaiter");
                    try
                    {
                        Task<object> final = (Task<object>)awaitMethod.Invoke(Coroutine.Current, new object[] { });
                        toReturn.Add(final.Result);
                        action(final.Result, index++);
                    }
                    catch (InvalidCastException)
                    {
                        try
                        {
                            var reflectedValue = Task.Run(() => (Task)awaitMethod.Invoke(Coroutine.Current, new object[] { }));
                            reflectedValue.Wait();
                        }
                        catch (Exception ex)
                        {
                            if (!(ex is AggregateException || ex is InvalidCastException)) throw;
                            var reflectedValue = awaitMethod.Invoke(Coroutine.Current, new object[] { });
                            if (reflectedValue != null)
                            {
                                toReturn.Add(reflectedValue);
                                action(reflectedValue, index++);
                            }
                        }
                    }
                    catch (NullReferenceException) { }
                    continue;
                }
                toReturn.Add(Coroutine.Current);
                action(Coroutine.Current, index++);
            }
            return toReturn.ToArray();
        }
    }
    #endregion

    #region Custom yieldable classes
    public abstract class CustomYieldable
    {
        public readonly bool Yield = false;

        protected CustomYieldable(bool yield)
        {
            Yield = yield;
        }
    }

    public class WaitForSeconds : CustomYieldable
    {
        private float Seconds;

        public WaitForSeconds(float Secs, bool yield = false) : base(yield)
        {
            Seconds = Secs;
        }

        public async Task GetAwaiter()
        {
            await Task.Delay((int)(Seconds * 1000));
        }
    }

    public class WaitUntil : CustomYieldable
    {
        private Func<bool> pred;

        public WaitUntil(Func<bool> predictate, bool yield = false) : base(yield)
        {
            pred = predictate;
        }

        public async Task GetAwaiter()
        {
            while (!pred()) await new WaitForSeconds(0f).GetAwaiter();
        }
    }

    public class WaitWhile : CustomYieldable
    {
        private Func<bool> pred;

        public WaitWhile(Func<bool> predictate, bool yield = false) : base(yield)
        {
            pred = predictate;
        }

        public async Task GetAwaiter()
        {
            while (pred()) await new WaitForSeconds(0f).GetAwaiter();
        }
    }

    public class WebQuery : CustomYieldable
    {
        public string Text { get; private set; }
        private string _url = "";
        public string URL
        {
            get => _url;
            set
            {
                Uri Result;
                if (!(Uri.TryCreate(value, UriKind.Absolute, out Result) && (Result.Scheme == Uri.UriSchemeHttp || Result.Scheme == Uri.UriSchemeHttps))) throw new ArgumentException("URL is not valid!");
                _url = value;
            }
        }

        public WebQuery(string url, bool yield = false) : base(yield)
        {
            URL = url;
        }

        public async Task<object> GetAwaiter()
        {
            Text = await new WebClient().DownloadStringTaskAsync(URL);
            return Text;
        }
    }
    #endregion
}