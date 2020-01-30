using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TDP.BaseServices.Infrastructure.Security;

namespace TDP.RSAKeyGenerator
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            TxtKey.Text = AsymmetricCryptography.CreateKey();
        }

        private void BtnGenerateForAppConfig_Click(object sender, EventArgs e)
        {
            TxtKey.Text = AsymmetricCryptography.CreateKey()
                            .Replace("<", "&lt;")
                            .Replace(">", "&gt;");
        }
    }
}
