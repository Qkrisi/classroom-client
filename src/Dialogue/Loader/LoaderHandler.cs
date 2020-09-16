namespace Classroom_Client
{
    public partial class Loader 
    {
        private double _percentage = 0;
        public double Percentage
        {
            get => _percentage;
            set
            {
                _percentage = value;
                label1.Text = $"Loading... {Percentage}%";
            }
        }

        public Loader(double percent)
        {
            InitializeComponent();
            Percentage = percent;
            label1.Font = new System.Drawing.Font(label1.Font.Name, 30f);
        }
    }
}