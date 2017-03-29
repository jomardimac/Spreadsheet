//Name: Jomar Dimaculangan
//ID:   11422439

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace CptS321 {


    public abstract class AbsCell : INotifyPropertyChanged {
        //need to be implemented for propertychanges to happen to  delagates;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //add a rowindex property (read only):
        private int _rowIndex;
        private int _colIndex;
        protected string _textCell;
        //references and referencedby:
        public List<AbsCell> references;
        public List<AbsCell> referencedBy;
        //background color:
        //private uint _bgcolor;
        private int _bgcolor;
        protected string _valCell;
        //added field: cell name:
        private string _cellname;
        //constructor:
        public AbsCell (int coli, int rowi) {
            _rowIndex = rowi;
            _colIndex = coli;
            //need these classes for referencing:
            references = new List<AbsCell>();
            referencedBy = new List<AbsCell>();
        }
        //property:
        public int GetRowI { get { return _rowIndex; } }
        public int GetColI { get { return _colIndex; } }

        public string GetCellName {
            get {
                //the only 'hard' part is converting the number to the proper cell col name:
                char col = (char) (_colIndex + 65);
                return Convert.ToString(col.ToString() + (_rowIndex + 1).ToString());
            }
        }

        public string Text {
            get { return _textCell; }
            set {
                //not changing, not doing anything:
                if (_textCell == value) {
                    PropertyChanged(this, new PropertyChangedEventArgs("Text"));
                    return;
                }
                _textCell = value;
                //update the propertychanged handdler and add textcellproperty
                PropertyChanged(this, new PropertyChangedEventArgs("Text"));
            }
        }

        //will need a to grab from text property that starts with '=';
        public string Val {
            get {
                //if the text doesn't have an equalsign, don't eval and just return
                //the string
                //if (_textCell[0] != '=') {
                //    return Text;

                //}
                return _valCell;
            }
            set {
                if (_valCell == value) {
                    PropertyChanged(this, new PropertyChangedEventArgs("Val")); return;
                }
                _valCell = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Val"));
            }
        }

        public int BGColor {
            get { return _bgcolor; }
            set {
                if (_bgcolor == value) {
                    PropertyChanged(this, new PropertyChangedEventArgs("BGColor"));
                    return;
                }
                _bgcolor = value;
                PropertyChanged(this, new PropertyChangedEventArgs("BGColor"));
            }
        }

        public void ClearReferences () {
            this.references.Clear();
        }

        public void ClearRefBy () {
            this.referencedBy.Clear();
        }

        public void RemoveRef(AbsCell cell) {
            this.references.Remove(cell);
        }
        public void RemoveRefBy(AbsCell cell) {
            this.referencedBy.Remove(cell);
        }
    }
}