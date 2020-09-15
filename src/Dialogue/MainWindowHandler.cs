using System.Collections.Generic;
using Classroom_Client.UI;

namespace Classroom_Client
{
    public partial class MainWindow
    {
        private List<CourseButton> CourseButtons = new List<CourseButton>();
        private int CourseY = 10;

        public MainWindow(object obj)
        {
            InitializeComponent();
        }

        public void OpenCourse(string ID)
        {

        }

        public void CreateCourseButton(CourseWrapper course)
        {
            CourseY += 10;
            var button = new CourseButton(course, CourseY);
            Controls.Add(button);
            CourseButtons.Add(button);
        }
    }
}