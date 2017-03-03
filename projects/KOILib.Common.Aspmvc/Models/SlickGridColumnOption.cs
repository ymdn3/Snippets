using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KOILib.Common.Aspmvc.Models
{
    /// <summary>
    /// SlickGrid カラム定義
    /// https://github.com/mleibman/SlickGrid/wiki/Column-Options
    /// </summary>
    public class SlickGridColumnOption
    {
        private dynamic _bag = new EvalstringBag();

        /// <summary>
        /// [default: null]	This accepts a function of the form function(cellNode, row, dataContext, colDef) and is used to post-process the cell’s DOM node / nodes
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string asyncPostRender
        {
            get { return _bag.asyncPostRender; }
            set { _bag.asyncPostRender = value; }
        }
        
        /// <summary>
        /// [default: null]	Used by the the slick.rowMoveManager.js plugin for moving rows. Has no effect without the plugin installed.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string behavior
        {
            get { return _bag.behavior; }
            set { _bag.behavior = value; }
        }

        /// <summary>
        /// [default: null]	In the "Add New" row, determines whether clicking cells in this column can trigger row addition. If true, clicking on the cell in this column in the "Add New" row will not trigger row addition.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cannotTriggerInsert
        {
            get { return _bag.cannotTriggerInsert; }
            set { _bag.cannotTriggerInsert = value; }
        }

        /// <summary>
        /// [default: ""]	Accepts a string as a class name, applies that class to every row cell in the column.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cssClass { get; set; }
        
        /// <summary>
        /// [default: true]	When set to true, the first user click on the header will do a ascending sort. When set to false, the first user click on the header will do an descending sort.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? defaultSortAsc { get; set; }
        
        /// <summary>
        /// [default: null]	The editor for cell edits {TextEditor, IntegerEditor, DateEditor…} See slick.editors.js
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string editor
        {
            get { return _bag.editor; }
            set { _bag.editor = value; }
        }

        /// <summary>
        /// [default: ""]	The property name in the data object to pull content from. (This is assumed to be on the root of the data object.)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string field { get; set; }
        
        /// <summary>
        /// [default: true]	When set to false, clicking on a cell in this column will not select the row for that cell. The cells in this column will also be skipped during tab navigation.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? focusable { get; set; }
        
        /// <summary>
        /// [default: null]	This accepts a function of the form function(row, cell, value, columnDef, dataContext) and returns a formatted version of the data in each cell of this column. For example, setting formatter to function(r, c, v, cd, dc) { return “Hello!”; } would overwrite every value in the column with “Hello!” See defaultFormatter in slick.grid.js for an example formatter.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string formatter
        {
            get { return _bag.formatter; }
            set { _bag.formatter = value; }
        }

        /// <summary>
        /// [default: null]	Accepts a string as a class name, applies that class to the cell for the column header.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string headerCssClass { get; set; }
        
        /// <summary>
        /// [default: ""]	A unique identifier for the column within the grid.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string id { get; set; }
        
        /// <summary>
        /// [default: null]	Set the maximum allowable width of this column, in pixels.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? maxWidth { get; set; }
        
        /// <summary>
        /// [default: 30]	Set the minimum allowable width of this column, in pixels.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? minWidth { get; set; }
        
        /// <summary>
        /// [default: ""]	The text to display on the column heading.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name { get; set; }
        
        /// <summary>
        /// [default: false]	If set to true, whenever this column is resized, the entire table view will rerender.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? rerenderOnResize { get; set; }
        
        /// <summary>
        /// [default: true]	If false, column can no longer be resized.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? resizable { get; set; }
        
        /// <summary>
        /// [default: true]	If false, when a row is selected, the CSS class for selected cells (“selected” by default) is not applied to the cell in this column.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? selectable { get; set; }
        
        /// <summary>
        /// [default: false]	If true, the column will be sortable by clicking on the header.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? sortable { get; set; }
        
        /// <summary>
        /// [default: ""]	If set to a non-empty string, a tooltip will appear on hover containing the string.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string toolTip { get; set; }

        /// <summary>
        /// Width of the column in pixels. (May often be overridden by things like minWidth, maxWidth, forceFitColumns, etc.)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? width { get; set; }

    }
}
