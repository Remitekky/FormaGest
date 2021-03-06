﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace GestionFormation
{
    public partial class ManagerSheet : Form
    {
        private DbGestionFormation db = new DbGestionFormation();
        private Session contextSession = new Session();
        private List<Participant> contextParticipant = new List<Participant>();
        private List<Candidater> contextCandidater = new List<Candidater>();

        public Session ContextSession { get; set; }

        public ManagerSheet(Session ContextSession)
        {
            InitializeComponent();
            contextSession = ContextSession;
            db.InitDb();
        }

        private void ManagerSheet_Load(object sender, EventArgs e)
        {
            contextCandidater = db.GetCandidatersAccepter(contextSession.Id.ToString());
            foreach (Candidater participant in contextCandidater)
            {
                contextParticipant = (db.GetParticipantById(participant.Participant_id.ToString()));
            }
            refreshDgv();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PdfPTable table = new PdfPTable(3);
            //actual width of table in points
            table.TotalWidth = 700f;
            //fix the absolute width of the table
            table.LockedWidth = true;

            //relative col widths in proportions - 1/3 and 2/3
            float[] widths = new float[] { 1f, 2f, 2f };
            table.SetWidths(widths);
            table.HorizontalAlignment = 0;

            PdfPCell cellName = new PdfPCell(new Phrase("Nom"));
            table.AddCell(cellName);
            PdfPCell cellFirstName = new PdfPCell(new Phrase("Prénom"));
            table.AddCell(cellFirstName);
            PdfPCell cellSign = new PdfPCell(new Phrase("Signature"));
            table.AddCell(cellSign);

            foreach (Participant participant in contextParticipant)
            {
                table.AddCell(participant.First_name);
                table.AddCell(participant.Name);
                table.AddCell("");
            }




            //Exporting to //
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "PDF file|*.pdf", ValidateNames = true })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A4.Rotate());
                    try
                    {
                        PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                        doc.Open();
                        doc.Add(table);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    finally
                    {
                        doc.Close();
                    }
                }
            }
        }

        private void refreshDgv()
        {
            dgvParticipants.DataSource = null;
            dgvParticipants.DataSource = contextParticipant;
            dgvParticipants.Columns[0].Visible = false;
            dgvParticipants.Columns[3].Visible = false;
            dgvParticipants.Columns[4].Visible = false;
            dgvParticipants.Columns[5].Visible = false;
            dgvParticipants.Columns[1].HeaderText = "Prénom";
            dgvParticipants.Columns[1].Width = 130;
            dgvParticipants.Columns[2].HeaderText = "Nom";
            dgvParticipants.Columns[2].Width = 140;
        }
    }
}
