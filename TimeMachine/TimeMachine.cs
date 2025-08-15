using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TimeMachine
{
    public partial class TimeMachine : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetSystemTime(ref SYSTEMTIME st);

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public ushort Year;
            public ushort Month;
            public ushort DayOfWeek;
            public ushort Day;
            public ushort Hour;
            public ushort Minute;
            public ushort Second;
            public ushort Milliseconds;
        }

        public TimeMachine()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void TimeMachine_Load(object sender, EventArgs e)
        {
            // Imposta la data attuale nel formato dd.MM.yyyy su label1
            if (label1 != null)
            {
                label1.Text = DateTime.Now.ToString("dd.MM.yyyy");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dateTimePicker1 == null || label1 == null)
            {
                MessageBox.Show("Il componente DateTimePicker o Label non è stato inizializzato.");
                return;
            }

            DateTime selectedDate = dateTimePicker1.Value;
            DateTime currentTime = DateTime.Now;

            // Combina la data selezionata con l'ora corrente
            SYSTEMTIME st = new SYSTEMTIME
            {
                Year = (ushort)selectedDate.Year,
                Month = (ushort)selectedDate.Month,
                Day = (ushort)selectedDate.Day,
                Hour = (ushort)currentTime.ToUniversalTime().Hour,                      //Il modulo .ToUniversalTime è stato inserito per
                Minute = (ushort)currentTime.ToUniversalTime().Minute,                  //risolvere un bug, in quanto insieme alla data
                Second = (ushort)currentTime.ToUniversalTime().Second,                  //Veniva cambiata anche l'ora. Essa veniva spostata
                Milliseconds = (ushort)currentTime.ToUniversalTime().Millisecond        //di due ore in avanti ad ogni modifica.
            };

            var processInfo = new ProcessStartInfo("cmd.exe", "/c whoami")
            {
                UseShellExecute = true,
                Verb = "runas"
            };

            try
            {
                Process.Start(processInfo)?.WaitForExit();

                if (!SetSystemTime(ref st))
                {
                    MessageBox.Show("Errore nel cambiare la data di sistema");
                }
                else
                {
                    // Conferma impostazione nuova data nel formato dd.MM.yyyy
                    MessageBox.Show($"Data di sistema cambiata con successo a {selectedDate:dd.MM.yyyy}");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Operazione annullata o errore nel richiedere i permessi di amministratore");
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
