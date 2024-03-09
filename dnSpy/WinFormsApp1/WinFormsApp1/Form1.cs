using ClassLibrary1;

namespace WinFormsApp1;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        string str = Class1.GetNumStr();

        MessageBox.Show(str);
    }
}
