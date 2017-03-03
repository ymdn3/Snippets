using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KOILib.Common.Aspmvc.Models
{
    /// <summary>
    /// SlickGrid オプション定義
    /// https://github.com/mleibman/SlickGrid/wiki/Grid-Options
    /// </summary>
    public class SlickGridOption
    {
        private dynamic _bag = new EvalstringBag();

        /// <summary>
        /// [defalt: false]  Makes cell editors load asynchronously after a small delay. This greatly increases keyboard navigation speed.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? asyncEditorLoading { get; set; }

        /// <summary>
        /// [default: 100]	Delay after which cell editor is loaded. Ignored unless asyncEditorLoading is true.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? asyncEditorLoadDelay { get; set; }

        /// <summary>
        /// [default: 50]
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? asyncPostRenderDelay { get; set; }

        /// <summary>
        /// [default: true]	Cell will not automatically go into edit mode when selected.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? autoEdit { get; set; }

        /// <summary>
        /// [default: false]	This disables vertical scrolling.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? autoHeight { get; set; }

        /// <summary>
        /// [default: "flashing"]	A CSS class to apply to flashing cells via flashCell().
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cellFlashingCssClass { get; set; }

        /// <summary>
        /// [default: "selected"]	A CSS class to apply to cells highlighted via setHighlightedCells().
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cellHighlightCssClass { get; set; }

        /// <summary>
        /// [default: null]
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string dataItemColumnValueExtractor
        {
            get { return _bag.dataItemColumnValueExtractor; }
            set { _bag.dataItemColumnValueExtractor = value; }
        }

        /// <summary>
        /// [default: 80]
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? defaultColumnWidth { get; set; }

        /// <summary>
        /// [default: defaultFormatter]
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string defaultFormatter
        {
            get { return _bag.defaultFormatter; }
            set { _bag.defaultFormatter = value; }
        }

        /// <summary>
        /// [default: false]
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? editable { get; set; }

        /// <summary>
        /// [default: queueAndExecuteCommand]	Not listed as a default under options in slick.grid.js
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string editCommandHandler
        {
            get { return _bag.editCommandHandler; }
            set { _bag.editCommandHandler = value; }
        }

        /// <summary>
        /// [default: null]	A factory object responsible to creating an editor for a given cell. Must implement getEditor(column).
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string editorFactory
        {
            get { return _bag.editorFactory; }
            set { _bag.editorFactory = value; }
        }

        /// <summary>
        /// [default: Slick.GlobalEditorLock]	A Slick.EditorLock instance to use for controlling concurrent data edits.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object editorLock { get; set; }

        /// <summary>
        /// [default: false]	If true, a blank row will be displayed at the bottom - typing values in that row will add a new one. Must subscribe to onAddNewRow to save values.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? enableAddRow { get; set; }

        /// <summary>
        /// [default: false]	If true, async post rendering will occur and asyncPostRender delegates on columns will be called.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? enableAsyncPostRender { get; set; }

        /// <summary>
        /// [default: null]	**WARNING**: Not contained in SlickGrid 2.1, may be deprecated
        /// </summary>
        [Obsolete("**WARNING**: Not contained in SlickGrid 2.1, may be deprecated", true)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object enableCellRangeSelection { get; set; }

        /// <summary>
        /// [default: true]	Appears to enable cell virtualisation for optimised speed with large datasets
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? enableCellNavigation { get; set; }

        /// <summary>
        /// [default: true]
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? enableColumnReorder { get; set; }
        
        /// <summary>
        /// [default: null]	**WARNING**: Not contained in SlickGrid 2.1, may be deprecated
        /// </summary>
        [Obsolete("**WARNING**: Not contained in SlickGrid 2.1, may be deprecated", true)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object enableRowReordering { get; set; }

        /// <summary>
        /// [default: false]
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? enableTextSelectionOnCells { get; set; }

        /// <summary>
        /// [default: false]	See: Example: Explicit Initialization
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? explicitInitialization { get; set; }

        /// <summary>
        /// [default: false]	Force column sizes to fit into the container (preventing horizontal scrolling). Effectively sets column width to be 1/Number of Columns which on small containers may not be desirable
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? forceFitColumns { get; set; }

        /// <summary>
        /// [default: false]
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? forceSyncScrolling { get; set; }

        /// <summary>
        /// [default: null]	A factory object responsible to creating a formatter for a given cell. Must implement getFormatter(column).
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string formatterFactory
        {
            get { return _bag.formatterFactory; }
            set { _bag.formatterFactory = value; }
        }

        /// <summary>
        /// [default: false]	Will expand the table row divs to the full width of the container, table cell divs will remain aligned to the left
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? fullWidthRows { get; set; }

        /// <summary>
        /// [default: 25]
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? headerRowHeight { get; set; }

        /// <summary>
        /// [default: false]
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? leaveSpaceForNewRows { get; set; }

        /// <summary>
        /// [default: false]	See: Example: Multi-Column Sort
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? multiColumnSort { get; set; }

        /// <summary>
        /// [default: true]
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? multiSelect { get; set; }

        /// <summary>
        /// [default: 25]
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? rowHeight { get; set; }

        /// <summary>
        /// [default: "selected"]
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string selectedCellCssClass { get; set; }

        /// <summary>
        /// [default: false]
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? showHeaderRow { get; set; }

        /// <summary>
        /// [default: false]	If true, the column being resized will change its width as the mouse is dragging the resize handle. If false, the column will resize after mouse drag ends.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? syncColumnCellResize { get; set; }

        /// <summary>
        /// [default: 25]
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? topPanelHeight { get; set; }
        
    }
}
