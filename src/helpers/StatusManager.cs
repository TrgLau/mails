using System;
using System.Drawing;
using System.Windows.Forms;

namespace TrgHelpers.StatusHelper
{

    public class StatusManager
    {
        private readonly Label _statusLabel;

        public StatusManager(Label statusLabel)
        {
            _statusLabel = statusLabel ?? throw new ArgumentNullException(nameof(statusLabel));
        }

        public void ShowSuccess(string message) => Update(message, Color.Green);
        public void ShowError(string message) => Update(message, Color.Red);
        public void ShowInfo(string message) => Update(message, Color.Blue);
        public void ShowWarning(string message) => Update(message, Color.OrangeRed);

        public void ShowReady(string message = "Prêt.") => Update(message, Color.Green);

        private void Update(string message, Color color)
        {
            if (_statusLabel.InvokeRequired)
            {
        
                _statusLabel.Invoke(new Action(() => {
                    _statusLabel.Text = message;
                    _statusLabel.ForeColor = color;
                }));
            }
            else
            {
            
                _statusLabel.Text = message;
                _statusLabel.ForeColor = color;
            }
        }
    }
}