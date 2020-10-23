using System.Collections.Generic;
using System.Windows.Forms;
using Classroom_Client.UI;

namespace Classroom_Client
{
    public partial class MainWindow
    {
        private FlowLayoutPanel CoursesPanel = new FlowLayoutPanel();
        private List<CourseButton> CourseButtons = new List<CourseButton>();
        private int CourseY = 10;

        public MainWindow(object obj)
        {
            InitializeComponent();
            Controls.Add(CoursesPanel);
        }

        public void OpenCourse(string ID)
        {  
        }

        public void CreateCourseButton(CourseWrapper course)
        {
            CourseY += CourseButtons.Count > 0 ? CourseButtons[CourseButtons.Count-1].Height + 10 : 0;
            var button = new CourseButton(course, CourseY);
            CoursesPanel.Controls.Add(button);
            CourseButtons.Add(button);
        }
    }
}