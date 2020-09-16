namespace Classroom_Client
{
    public partial class Loader
    {
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label1.Image = null;
            this.label1.Text = "Loading... 0%";
            this.label1.BackColor = System.Drawing.SystemColors.ControlText;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(616, 352);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(356, 175);
            this.label1.TabIndex = 0;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(1912, 1053);
            this.Text = "Classroom Client";
            this.Controls.Add(this.label1);
            this.Name = "Classroom Client";
        }
        private System.Windows.Forms.Label label1;
    }
}
