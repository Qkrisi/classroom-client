using static Classroom_Client.ClassroomData;

namespace Classroom_Client.UI
{
    public class CourseButton : LinkedButton<CourseWrapper>
    {
        public CourseButton(CourseWrapper course, int Y) : base(course)
        {
            Click += (a, e) => Instance.OpenCourse(LinkedData.course.Id);
            using (var cg = CreateGraphics())
            {
                Text = LinkedData.course.Name;
                Height = 50;
                Width = (int)cg.MeasureString(Text, Font).Width;
                Location = new System.Drawing.Point(40, Y);
            }
        }
    }
}