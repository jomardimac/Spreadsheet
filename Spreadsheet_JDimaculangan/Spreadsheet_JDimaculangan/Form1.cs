//Name: Jomar Dimaculangan
//ID:   11422439

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using CptS321;


namespace Spreadsheet_JDimaculangan {

    public partial class Form1 : Form {
        //create a spreadsheet and initilize it with 26 columns and rows:
        private Spreadsheet grid = new Spreadsheet(26, 50);
        public Form1 () {
            InitializeComponent();
        }


        private void Form1_Load (object sender, EventArgs e) {

            //go through a forloop to add each names
            int i = 0;
            for (char a = 'A'; a <= 'Z'; a++, i++) {
                string b = a.ToString();
                dataGridView1.Columns.Add(b, b);
                dataGridView1.AutoResizeColumn(i);
            }
            //adds 50 rows
            this.dataGridView1.Rows.Add(50);
            //go through for loop to add each name:
            for (int j = 0; j < 50; j++) {
                string b = (j + 1).ToString();
                dataGridView1.Rows [j].HeaderCell.Value = b;

            }
            //update each cells in the grid:for (int i = 0; i < cols; i++) {
            for (int k = 0; k < grid.ColCount; k++) {
                for (int j = 0; j < grid.RowCount; j++) {
                    //subscribe each cell to the spreadsheet proprerty chagned
                    grid.GetCell(k, j).PropertyChanged += SpreadPropertyChanged;
                }
            }
            //subscribe for cell end and begin:
            dataGridView1.CellBeginEdit += datagridview1_CellBeginEdit;
            dataGridView1.CellEndEdit += datagridview1_CellEndEdit;

            //disable undo and redo buttons at the beginning:
            undoChangingCellBackgroundColorToolStripMenuItem.Enabled = false;
            redoCellTextChangeToolStripMenuItem.Enabled = false;
            //if either one is empty, it'll be false, if something is in the stack, then it will be true:

        }

        //first part of hw7: Cell begin and cell end edit:
        private void datagridview1_CellBeginEdit (object sender, DataGridViewCellCancelEventArgs e) {
            //get the changed value from grid:=

            string changed = grid.GetCell(e.ColumnIndex, e.RowIndex).Text;
            //update the gui's:

            dataGridView1.Rows [e.RowIndex].Cells [e.ColumnIndex].Value = changed;
        }

        private void datagridview1_CellEndEdit (object sender, DataGridViewCellEventArgs e) {
            grid.UndoRedo.clearRedo();
            //HW8: UNDO REDO HERE: 
            List<IInvertibleCmd> undoCmds = new List<IInvertibleCmd>();
            int rows = e.RowIndex;
            int col = e.ColumnIndex;
            AbsCell changedCell = grid.GetCell(col, rows);

            //if user doesn't put anything in the cell:
            if (dataGridView1.Rows [rows].Cells [col].Value == null) {
                string oldT = changedCell.Text;
                grid.GetCell(col, rows).Text = "";
                dataGridView1.Rows [rows].Cells [col].Value = "";
                undoCmds.Add(new RestoreText(oldT, changedCell));
                grid.AddUndo(new MultiCmd("", undoCmds));
            }
            else {
                //save the new text and push it for updating menu
                var newText = dataGridView1.Rows [e.RowIndex].Cells [col].Value.ToString();
                //save the old text before it gets changed:
                string changed = changedCell.Text;
                undoCmds.Add(new RestoreText(changedCell.Text, changedCell));
                changedCell.Text = newText; //user's newly input
                dataGridView1.Rows [rows].Cells [col].Value = changedCell.Val; //show it in the screen
                grid.AddUndo(new MultiCmd(changedCell.Text, undoCmds));
            }

            EnableDisableUndoRedo();
        }

        private void dataGridView1_CellContentClick (object sender, DataGridViewCellEventArgs e) {

        }

        //Create a button on the form that shows a demo:

        //private void button1_Click (object sender, EventArgs e) {
        //should set text in 50 random cells toa text string "hello world"
        //Random rand = new Random();
        //string textsample = "Hellow World!";
        //for (int i = 1; i <= 50; i++) {
        //    grid.GetCell(rand.Next(26), rand.Next(50)).Text = textsample;
        //}
        ////Do a loop to set the text in every cell in the columb B to This is cell B#"
        //for (int i = 1; i <= 50; i++) {
        //    grid.GetCell(1, i).Text = "This is cell B" + (i).ToString();
        //}
        ////Set text in every cell in columb a to "=B#":
        //for (int i = 1; i <= 50; i++) {
        //    string textname = "=B" + i.ToString();
        //    grid.GetCell(0, i).Text = textname;
        //}

        //}

        //Implement this so that when a cell's value changes, it gets updated in the cell in the gridview:
        private void SpreadPropertyChanged (object sender, PropertyChangedEventArgs e) {
            //instantiate the cell:
            AbsCell cellCh = (sender as AbsCell);
            if (cellCh != null) {
                if (e.PropertyName == "Val" || e.PropertyName == "Text") {
                    //Datagridview has rows and cells which has a value property. 
                    dataGridView1.Rows [cellCh.GetRowI].Cells [cellCh.GetColI].Value = cellCh.Val;

                }
                //change the color:
                if (e.PropertyName == "BGColor") {
                    //dataGridView1.Rows[cellCh.GetRowI].Cells[cellCh.GetColI].Style.BackColor = Color.FromArgb((int) cellCh.BGColor);
                    //help from old's email:
                    Color bgcol = Color.FromArgb(cellCh.BGColor);
                    if (bgcol.A == 0 && bgcol.R == 0 && bgcol.G == 0 && bgcol.B == 0) {
                        dataGridView1.Rows [cellCh.GetRowI].Cells [cellCh.GetColI].Style.BackColor = Color.White;
                    }
                    else {
                        dataGridView1.Rows [cellCh.GetRowI].Cells [cellCh.GetColI].Style.BackColor = Color.FromArgb(255, bgcol.R, bgcol.G, bgcol.B);
                    }
                }
            }

            //int row = cellCh.GetRowI;
            //int cell = cellCh.GetColI;

            //dataGridView1.Rows[row].Cells[cell].Value = cellCh.Val;
        }

        //change color of selected cells:
        private void changeBackgroundColorToolStripMenuItem_Click (object sender, EventArgs e) {

            //from msdn help with color strip menu:
            DialogResult result = colorDialog1.ShowDialog();
            //grab the value of selected color using ToArgb();
            int colorint = colorDialog1.Color.ToArgb();
            int oldColor;
            List<uint> newCol = new List<uint>();
            //AbsCell cellCh = (sender as AbsCell);
            List<IInvertibleCmd> undoCmds = new List<IInvertibleCmd>();
            if (result == DialogResult.OK) {
                //go through the selected cells:
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++) {
                    //faster references:
                    int row = dataGridView1.SelectedCells [i].RowIndex;
                    int col = dataGridView1.SelectedCells [i].ColumnIndex;
                    AbsCell changedCell = grid.GetCell(col, row);
                    oldColor = changedCell.BGColor;
                    //restore old color:
                    undoCmds.Add(new RestoreBGColor(oldColor, grid.GetCell(col, row)));
                    //pass through the cell:
                    grid.GetCell(col, row).BGColor = colorint;
                }

                grid.AddUndo(new MultiCmd("Changed background color", undoCmds, colorint));
                EnableDisableUndoRedo();
            }
        }

        private void cellToolStripMenuItem_Click (object sender, EventArgs e) {

        }

        private void menuStrip1_ItemClicked (object sender, ToolStripItemClickedEventArgs e) {

        }

        //undo button:
        private void undoChangingCellBackgroundColorToolStripMenuItem_Click (object sender, EventArgs e) {
            grid.UndoRedo.undo();

            EnableDisableUndoRedo();
        }

        //redu button:
        private void redoCellTextChangeToolStripMenuItem_Click (object sender, EventArgs e) {

            grid.UndoRedo.redo();
            EnableDisableUndoRedo();
        }

        public void EnableDisableUndoRedo () {
            //undoChangingCellBackgroundColorToolStripMenuItem.Enabled = grid.UndoRedo.canUndo() != false;
            //redoCellTextChangeToolStripMenuItem.Enabled = grid.UndoRedo.canRedo() != false;

            if (grid.UndoRedo.canUndo() == true) {
                undoChangingCellBackgroundColorToolStripMenuItem.Enabled = true;
                undoChangingCellBackgroundColorToolStripMenuItem.Text = "Undo: " + grid.UndoRedo.UndoText();

            }
            else {
                undoChangingCellBackgroundColorToolStripMenuItem.Enabled = false;
                undoChangingCellBackgroundColorToolStripMenuItem.Text = "Undo grey";
            }
            if (grid.UndoRedo.canRedo() == true) {
                redoCellTextChangeToolStripMenuItem.Enabled = true;
                redoCellTextChangeToolStripMenuItem.Text = "Redo: " + grid.UndoRedo.RedoText();
            }
            else {
                redoCellTextChangeToolStripMenuItem.Enabled = false;
                redoCellTextChangeToolStripMenuItem.Text = "Redo grey";
            }
        }

        private void saveToolStripMenuItem_Click (object sender, EventArgs e) {
            //remember to save in a file:
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "All files (*.*)|*.*";
            //make sure it pops up and says ok not cancel:
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                //evan wants the stream so make it a stream of file name and open it as a save file:
                Stream stream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                //save:
                grid.Save(stream);
                stream.Dispose();
            }
            
        }

        private void loadToolStripMenuItem_Click (object sender, EventArgs e) {
            //literally similar to save but with load.
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                //the only different thing right here. open filename to open not create and read:
                Stream stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                grid.Read(stream);
                stream.Dispose();
            }
            EnableDisableUndoRedo();
        }
    }
}
