//Name: Jomar Dimaculangan
//ID:   11422439

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
//going to be using xdocument:
using System.Xml.Linq;

namespace CptS321 {
    //this class will serve as a container for a 2d array of cells.
    //
    public class Spreadsheet : INotifyPropertyChanged {

        //create a cellpropertychangedhander that will subsribe from events
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        //create a cell class that inherits from spreadsheetcell:
        public class Cell : AbsCell {
            //added in HW7: EXPTREES FOR EQUATIONS:
            //public ExpTree expression = new ExpTree("");
            //constructor
            public Cell (int coli, int rowi) : base(coli, rowi) { }

            //now I can set values here:
            public void setVal (string newVal) {
                _valCell = newVal;
            }
        }

        //create a 2 day array of cell
        private AbsCell [,] _sheet;
        public Cell m_Cell;

        //for the getcell:
        private int _rows;
        private int _cols;
        //returns column and row count:
        public int ColCount { get { return _cols; } }
        public int RowCount { get { return _rows; } }

        //for undo redo stack:
        private UndoRedoSystem _undoredo = new UndoRedoSystem();

        //should allocate a 2d array
        public Spreadsheet (int cols, int rows) {
            //allocate the cell.
            _rows = rows;
            _cols = cols;
            _sheet = new AbsCell [cols, rows];

            //time to create the cell by putting rows and columns:
            for (int i = 0; i < cols; i++) {
                for (int j = 0; j < rows; j++) {
                    _sheet [i, j] = new Cell(i, j);
                    //subscribe to cellpropertychange 
                    //sheet at i,j = cell
                    _sheet [i, j].PropertyChanged += CellPropertyChanged;
                }
            }
        }

        public void ClearAllCells () {
            //time to create the cell by putting rows and columns:
            for (int i = 0; i < ColCount; i++) {
                for (int j = 0; j < RowCount; j++) {
                    //check if has something:
                    if (_sheet[i, j].Text != null || _sheet[i,j].BGColor != 0) {
                        _sheet[i, j].Text = "";
                        _sheet[i, j].BGColor = 0;
                    }
                }
            }
        }
        //getcell function:
        public AbsCell GetCell (int cols, int rows) {
            if (_sheet [cols, rows] == null) {
                return null;
            }
            return _sheet [cols, rows];
        }


        public void AddUndo (MultiCmd pushundo) {
            this.UndoRedo.AddUndo(pushundo);
        }
        //getundoredo:
        public UndoRedoSystem UndoRedo {
            get { return _undoredo; }
        }

        public void Save(Stream doc) {
            
            //create a document with spreadsheet as an element name: 
            //itll be like this: <spreadsheet> </spreadsheet>
            XDocument document = new XDocument(
                new XElement("Spreadsheet"));
            

            //go through each cell:
            foreach (AbsCell cell in _sheet) {
                //make sure ther was changes:
                if (cell.Text != null || cell.BGColor != 0) {
                    //get cell name:
                    document.Root.Add(new XElement("Cell", new XAttribute("name", cell.GetCellName)));
                }

            }
            //inside that cellname: get its attributes aka bgcolor and value/text: 
            foreach (XElement element in document.Descendants("Cell")) {
                //gets each cell:
                string cellname = Convert.ToString(element.Attribute("name").Value);
                int col = Convert.ToChar(cellname [0]) - 65;
                //grab row:
                int row = Convert.ToInt32(cellname.Substring(1)) - 1;
                element.Add(new XElement("bgcolor", _sheet[col, row].BGColor));
                element.Add(new XElement("Text", _sheet [col, row].Text));
            }
            

            //save:
            document.Save(doc);
            doc.Position = 0;
        }


        //loading/reading:
        public void Read(Stream filename) {
            //clear all cells:
            ClearAllCells();
            //open up that xml thoo:
            XDocument document = XDocument.Load(filename);

            //parse:
            foreach (XElement element in document.Root.Elements()) {
                //grab the cellname and grab its col,row:
                string cellname = element.Attribute("name").Value;
                int col = Convert.ToChar(cellname [0]) - 65;
                int row = Convert.ToInt32(cellname.Substring(1)) - 1;
                AbsCell cell = GetCell(col, row);
                //put in bgcolor & text:
                if (cell != null) {
                    
                    var cellBgColor = element.Element("bgcolor");
                    //check if not nothing:
                    if (cellBgColor != null) {
                        cell.BGColor = int.Parse(cellBgColor.Value);
                    }

                    var cellText = element.Element("Text");
                    //check if there's something:
                    if (cellText != null) {
                        cell.Text = cellText.Value;
                    }
                }
            }
            
        }
        
        //HW10: out of bounce:
        private bool TextOutOfBounds(string s) {
            bool result = false;
            int colIndex = Convert.ToChar(s [0]) - 65;
            //grab row:
            try {
                int rowIndex = Convert.ToInt32(s.Substring(1)) - 1;

                if (rowIndex < 50) {
                    result = true;
                }
            }
            catch {
                result = false;

            }
            return result;
        }
        //self ref:
        private bool CellSelfRef(AbsCell celln) {
            return celln.GetCellName == celln.Text.Substring(1);
        }

        //circulear referencing:
        private bool CellCircularReference(AbsCell cell) {
            bool result = false;

            return result;
        }
        //need to have a cell change:
        //cell's property change was from change in text and value.
        private void CellPropertyChanged (object sender, PropertyChangedEventArgs e) {
            //check if text is changed:
            Cell changedCell = (sender as Cell);
            ////background color changed:
            if (e.PropertyName == "BGColor") {
                //throw in an event handler:
                this.PropertyChanged(sender, new PropertyChangedEventArgs("BGColor"));
            }
            else if ("Text" == e.PropertyName) {
                
                if (changedCell.Text == null || changedCell.Text == "") {
                    changedCell.Val = changedCell.Text;
                }
                
                //literally just make val = text if it doesn't have any '=' sign
                else if (changedCell.Text.Length > 0 && changedCell.Text [0] != '=') {
                    
                    //just make cellval = text if it aint an equation:
                    changedCell.ClearReferences();
                    changedCell.Val = changedCell.Text;

                    //used on an equation tho:
                    if (changedCell.referencedBy.Count > 0) {
                        //we can now put it as an expression:
                        ExpTree exp = new ExpTree(changedCell.Text);
                        //just set value to the text
                        changedCell.setVal(exp.Eval().ToString());
                        try {
                            foreach (Cell refCell in changedCell.referencedBy) {
                                //tryna figure out to just do 
                                string temp = refCell.Text;

                                refCell.Text = temp;
                                //refCell.Text = "Hello";
                            }
                        }
                        catch {
                        }
                    }
                    

                }
                //will now evulate the formula for the val:
                else if (changedCell.Text.Length > 0 && changedCell.Text [0] == '=') {
                    //text out of boundS:
                    if (this.TextOutOfBounds(changedCell.Text.Substring(1)) == false) {
                        changedCell.Text = "!(Bad reference)";
                        changedCell.Val = changedCell.Text;
                        return;
                    }
                    //self reference:
                    else if (this.CellSelfRef(changedCell) == true) {
                        changedCell.Text = "!(self reference)";
                        changedCell.Val = changedCell.Text;
                        return;
                    }
                    else {
                        //grab equation after equal sign:
                        ExpTree expression = new ExpTree(changedCell.Text.Substring(1));
                        //grab all variables in that expression:
                        List<string> cellList = expression.grabVars();
                        //go through each variables in the expression and update it:
                        foreach (string s in cellList) {
                            //make a value to set in that expression:
                            double val = 0;
                            //first char always gonna be from A-Z
                            int colIndex = Convert.ToChar(s[0]) - 65;
                            //grab row:
                            int rowIndex = Convert.ToInt32(s.Substring(1)) - 1;
                            val = Convert.ToDouble(_sheet[colIndex, rowIndex].Val);
                            changedCell.references.Add(_sheet[colIndex, rowIndex]);
                            _sheet[colIndex, rowIndex].referencedBy.Add(changedCell);
                            expression.SetVar(s, val);
                        }
                        //Now evaluate what was in the expression tree before going through the refby list:
                        changedCell.setVal(expression.Eval().ToString());
                        //go through the references and update each referenced by:
                        try {
                            foreach (Cell refCell in changedCell.referencedBy) {
                                //tryna figure out to just do 
                                string temp = refCell.Text;

                                refCell.Text = temp;
                                //refCell.Text = "Hello";
                            }
                        }
                        catch {
                        }



                        //set that expression to the value:
                        //changedCell.setVal(expression.Eval().ToString());
                    }
                }
            }

        }
    }
}
