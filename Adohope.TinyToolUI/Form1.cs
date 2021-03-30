using Adohope.Shared.Extractor;
using System;
using System.Windows.Forms;

namespace Adohope.TinyToolUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string BackupPath
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }

        public Backup Backup { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    BackupPath = fbd.SelectedPath;
                    panel1.Enabled = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                panel1.Enabled = false;
                button1.Enabled = false;

                var outputFolderPath = string.Empty;

                using (var fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select output folder";
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        outputFolderPath = fbd.SelectedPath;
                    }
                    else
                    {
                        MessageBox.Show("Please select output folder to continue extracting data");
                        return;
                    }
                }

                Backup = new Backup(BackupPath);

                if(chkBox_Photos.Checked)
                {
                    var assetService = Backup.GetAssetService();

                    var assets = assetService.GetAllAssets();

                    //
                    Extractor extractor = new Extractor(Backup, outputFolderPath);
                    extractor.CreateUniqueFolderForTheExportedData();
                    extractor.ExtractPhotosAndVideos(assets);
                }

                if(chkBox_VoiceNotes.Checked)
                {
                    try
                    {
                        var recordingService = Backup.GetRecordingService();

                        var voiceNotes = recordingService.GetAllRecordings();

                        Extractor extractor = new Extractor(Backup, outputFolderPath);
                        extractor.CreateUniqueFolderForTheExportedData();
                        extractor.ExtractVoiceNotes(voiceNotes);
                    }
                    catch (Exception)
                    {
                        // sometime database is empty, founded in older version of 8.*
                    }
                }

                if(chkBox_Messages.Checked)
                {
                    var chatService = Backup.GetChatService();

                    var chats = chatService.GetAllChats();

                    Extractor extractor = new Extractor(Backup, outputFolderPath);
                    extractor.CreateUniqueFolderForTheExportedData();
                    extractor.ExtractChats(chats);
                }

                if(chkBox_Contacts.Checked)
                {
                    var personService = Backup.GetPersonService();

                    var contacts = personService.GetAllPersons();

                    Extractor extractor = new Extractor(Backup, outputFolderPath);
                    extractor.CreateUniqueFolderForTheExportedData();
                    extractor.ExtractContacts(contacts);
                }

                if(chkBox_Notes.Checked)
                {
                    var noteService = Backup.GetNoteService();

                    try
                    {
                        var notes = noteService.GetAllNotes();

                        Extractor extractor = new Extractor(Backup, outputFolderPath);
                        extractor.CreateUniqueFolderForTheExportedData();
                        extractor.ExtractNotes(notes);
                    }
                    catch (Exception)
                    {
                        // sometime database is empty, founded in older version of 8.*
                    }
                }

                MessageBox.Show("Extracting done..");

                panel1.Enabled = true;
                button1.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
