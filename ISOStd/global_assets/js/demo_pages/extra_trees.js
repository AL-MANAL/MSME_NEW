/* ------------------------------------------------------------------------------
 *
 *  # Fancytree
 *
 *  Demo JS code for extra_trees.html page
 *
 * ---------------------------------------------------------------------------- */


// Setup module
// ------------------------------



var Fancytree = function () {


    //
    // Setup module components
    //

    // Uniform
    var _componentFancytree = function () {

        var results;
        var roleid = document.getElementById("role_id").value;
        var branch_id = document.getElementById("branch_id").value;
        $.ajax({
            url: summary.summaryUrl,
            type: 'POST',
            dataType: "json",
            data: { 'RoleId': roleid, 'branch_id': branch_id },
            success: function (result) {
                results = result;

                $('.tree-table').fancytree({
                    extensions: ['table'],
                    //checkbox: true,
                    table: {
                        indentation: 10,      // indent 20px per node level
                        nodeColumnIdx: 1,     // render the node title into the 2nd column
                        //checkboxColumnIdx: 0  // render the checkboxes into the 1st column
                    },
                    source:
                      [
            //{ "title": "Events" },
            {
              "title": "Documents Management", "folder": true, "expanded": true, "children":
               [
                       { "title": "Internal Documents" },
                       { "title": "External Documents" },
                       { "title": "Records management" },
                       { "title": "Document Change Request" },
                       { "title": "Documents status tracking" }
               ]
            },
            {
            	"title": "Objectives/KPIs", "folder": true, "expanded": true, "children":
               [
                       { "title": "Objectives & targets" },
					   { "title": "KPIs" },					   
               ]
             },
             {
                         "title": "Legal Compliance", "folder": true, "expanded": true, "children":
                      [
                           { "title": "Legal Register" },
                           { "title": "Certificates" },
                      ]
              },
             {
            	"title": "Context of Organization", "folder": true, "expanded": true, "children":
               [
                       { "title": "Needs & Expectations-Interested Parties" },                      
					   { "title": "Internal and External Issues" },					   
               ]
            },
           
             {
             	"title": "Risks & Opportunities", "folder": true, "expanded": true, "children":
                [      
                        { "title": "Qulaity Related Risk" },
                        { "title": "Health&Safety/Envionmental Risks Assesment" },
						{ "title": "Job Safety Analysis(JSA)" },
						//{ "title": "Resource Consumption List" },
                ]
            },
              {
                  "title": "Nonconformance & Corrective Action", "folder": true
             },
             {
             	"title": "Portal", "folder": true, "expanded": true, "children":
                [      
                        { "title": "Sub-CR Master" },
                          { "title": "Access Request-Government" },
                          { "title": "Portal Access User Log" },						
                ]
             },
              {
                  "title": "Human Resources", "folder": true, "expanded": true, "children":
                 [
                         { "title": "Employee Mgmt" },
                         { "title": "Employee Performance Evaluation" },
                         { "title": "Competancy Evaluation" },
                         //{ "title": "Exit Employee List" },
                         { "title": "Visitors" },
                 ]
                            },
                            {
                                "title": "Trainings", "folder": true, "expanded": true, "children":
                                    [
                                        { "title": "Add Training Topic" },
                                        { "title": "Perform Training" },
                                        { "title": "Training Lists" },
                                    ]
                            },
               {
                  "title": "Change Management Request", "folder": true
              },
               {
                   "title": "Actions", "folder": true, "expanded": true, "children":
                  [
                          { "title": "Action Tracking Register" },
                          { "title": "HSE Actions Tracking Register" },
                  ]
               },
               {
                   "title": "Meetings", "folder": true, "expanded": true, "children":
                [
                     { "title": "Meeting Type/Agenda" },
                     { "title": "Schedule Meeting" },
                     { "title": "Unplanned Meeting" },
                ]
               },
               {
                   "title": "Suppliers Management", "folder": true, "expanded": true, "children":
                [
                     { "title": "Suppliers" },
                     { "title": "Reporting Discrepancies" },
                     { "title": "Supplier Performance" },
                     //{ "title": "Approved Ext Provider List" },
                     { "title": "Performance Evaluation" },
                ]
               },
                {
                    "title": "Customer", "folder": true, "expanded": true, "children":
                 [
                      { "title": "Visits" },
                      { "title": "Add Customer/Send Feedback" },
                      { "title": "Complaints" },
                      { "title": "Upload Survey" },
                      { "title": "Customer Property Log" },
                 ]
                },
                 
                   {
                       "title": "Audits", "folder": true, "expanded": true, "children":
                    [
                         { "title": "Audit Process" },
                         { "title": "External Audit" },
                         //{ "title": "External Audit Report" },
                         { "title": "Customer Audit" },
                        // { "title": "Raise Nc" },
                         //{ "title": "Audit Process Checklist(With Upload)" },
                         { "title": "Generate Audit Checklist" },
                    ]
                            },
                   {
                        "title": "Measuring Equiptments", "folder": true, "expanded": true, "children":
                     [
                          { "title": "Machinery" },
                          { "title": "Tools & Gauges" },
                     ]
                    },  
                    {
                        "title": "HSE", "folder": true, "expanded": true, "children":
                            [
                                { "title": "HSE Inspection" },
                                { "title": "Incident Reports" },
                                { "title": "Toolbox Talks Log And Report" },
                                { "title": "Safety Violation Log And Report" },
                                { "title": "Emergency Plan And Record" },
                                { "title": "PPE Issue Log" },  
                          //{ "title": "Near Miss Reporting" },                                                  
                          //{ "title": "Air/Water/Noise Quality Survey" },
                          //{ "title": "Waste Management" },
                          //{ "title": "Safety Observation Card" },
                          //{ "title": "HSE Induction" },
                          //{ "title": "First Aid Box" },
                          //{ "title": "Fire Equipment Inspection" },
                     ]
                    },
                                       
                     //{
                     //    "title": "Trainings", "folder": true, "expanded": true, "children":
                     // [
                     //      { "title": "TrainingList" },
                     // ]
                     //},
                      {
                          "title": "Work Permits", "folder": true, "expanded": true, "children":
                       [
                            { "title": "Access Work Permit" },
                            { "title": "Work Permit" },
                       ]
                      }//,
                      //{
                      //    "title": "Settings", "folder": true, "expanded": true, "children":
                      // [
                      //      { "title": "Company Profile" },
                      //      { "title": "Department" },
                      //      { "title": "User Profile" },
                      //      { "title": "Mgmt Dropdown Data" },
                      //      { "title": "Role" },
                      //      { "title": "ISO Standards" },
                      // ]
                      //}
                      ],

                    renderColumns: function (event, data) {
                        var node = data.node,
                        $tdList = $(node.tr).find('>td');

                        $tdList.eq(0).text(node.getIndexHier()).addClass('alignRight');


                        //-------------Start Documents Management----------------
                        if (node.title == "Documents Management") {

                            if (results.Documents.includes("1")) {
                                $tdList.eq(2).addClass('text-center').html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Documents"  onclick="setDocument(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).addClass('text-center').html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Documents" onclick="setDocument(' + results.Id_access + ')" />');
                            }
                        }
                        if (node.title == "Internal Documents") {

                            if (results.Documents.includes("0")) {
                                $tdList.eq(2).addClass('text-center').html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="InternalDoc"  onclick="setInternalDoc(' + results.Id_access + ')" />');
                                $tdList.eq(3).addClass('text-center').html('<input type="checkbox" class="form-input-styled" disabled id="select" name="InternalDoc ' + results.Id_access + '" value="2" onclick="setInternalDocR(' + results.Id_access + ')" />');
                                $tdList.eq(4).addClass('text-center').html('<input type="checkbox" class="form-input-styled" disabled id="select" name="InternalDoc ' + results.Id_access + '" value="3" onclick="setInternalDocR(' + results.Id_access + ')" />');
                                $tdList.eq(5).addClass('text-center').html('<input type="checkbox" class="form-input-styled" disabled id="select" name="InternalDoc ' + results.Id_access + '" value="4"  onclick="setInternalDocR(' + results.Id_access + ')" />');
                                $tdList.eq(6).addClass('text-center').html('<input type="checkbox" class="form-input-styled" disabled id="select" name="InternalDoc ' + results.Id_access + '" value="5" onclick="setInternalDocR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.InternalDoc.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="InternalDoc"  onclick="setInternalDoc(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="InternalDoc"  onclick="setInternalDoc(' + results.Id_access + ')" />');
                                }
                                if (results.InternalDoc.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="InternalDoc ' + results.Id_access + '" value="2" onclick="setInternalDocR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).addClass('text-center').html('<input type="checkbox" class="form-input-styled" id="select" name="InternalDoc ' + results.Id_access + '" value="2"  onclick="setInternalDocR(' + results.Id_access + ')" />');
                                }
                                if (results.InternalDoc.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="InternalDoc ' + results.Id_access + '" value="3"  onclick="setInternalDocR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="InternalDoc ' + results.Id_access + '" value="3"  onclick="setInternalDocR(' + results.Id_access + ')" />');
                                }
                                if (results.InternalDoc.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="InternalDoc ' + results.Id_access + '" value="4"  onclick="setInternalDocR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="InternalDoc ' + results.Id_access + '" value="4"  onclick="setInternalDocR(' + results.Id_access + ')" />');
                                }
                                if (results.InternalDoc.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled"  id="select" checked name="InternalDoc ' + results.Id_access + '" value="5"  onclick="setInternalDocR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="InternalDoc ' + results.Id_access + '" value="5"  onclick="setInternalDocR(' + results.Id_access + ')" />');
                                }

                            }
                        }
                        if (node.title == "External Documents") {
                            if (results.Documents.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="ExternalDoc"  onclick="setExternalDoc(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ExternalDoc ' + results.Id_access + '" value="2"  onclick="setExternalDocR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ExternalDoc ' + results.Id_access + '" value="3"  onclick="setExternalDocR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ExternalDoc ' + results.Id_access + '" value="4"  onclick="setExternalDocR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ExternalDoc ' + results.Id_access + '" value="5"  onclick="setExternalDocR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.ExternalDoc.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="ExternalDoc"  onclick="setExternalDoc(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="ExternalDoc"  onclick="setExternalDoc(' + results.Id_access + ')" />');
                                }
                                if (results.ExternalDoc.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ExternalDoc ' + results.Id_access + '" value="2"  onclick="setExternalDocR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="ExternalDoc ' + results.Id_access + '" value="2"  onclick="setExternalDocR(' + results.Id_access + ')" />');
                                }
                                if (results.ExternalDoc.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ExternalDoc ' + results.Id_access + '" value="3"  onclick="setExternalDocR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="ExternalDoc ' + results.Id_access + '" value="3"  onclick="setExternalDocR(' + results.Id_access + ')" />');
                                }
                                if (results.ExternalDoc.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ExternalDoc ' + results.Id_access + '" value="4"  onclick="setExternalDocR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="ExternalDoc ' + results.Id_access + '" value="4"  onclick="setExternalDocR(' + results.Id_access + ')" />');
                                }
                                if (results.ExternalDoc.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ExternalDoc ' + results.Id_access + '" value="5"  onclick="setExternalDocR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="ExternalDoc ' + results.Id_access + '" value="5"  onclick="setExternalDocR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Records management") {

                            if (results.Documents.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Record"  onclick="setRecord(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Record ' + results.Id_access + '" value="2"  onclick="setRecordR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Record ' + results.Id_access + '" value="3"  onclick="setRecordR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Record ' + results.Id_access + '" value="4"  onclick="setRecordR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Record ' + results.Id_access + '" value="5"  onclick="setRecordR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Record.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Record"  onclick="setRecord(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Record"  onclick="setRecord(' + results.Id_access + ')" />');
                                }
                                if (results.Record.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Record ' + results.Id_access + '" value="2"  onclick="setRecordR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Record ' + results.Id_access + '" value="2"  onclick="setRecordR(' + results.Id_access + ')" />');
                                }
                                if (results.Record.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Record ' + results.Id_access + '" value="3"  onclick="setRecordR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Record ' + results.Id_access + '" value="3"  onclick="setRecordR(' + results.Id_access + ')" />');
                                }
                                if (results.Record.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Record ' + results.Id_access + '" value="4"  onclick="setRecordR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Record ' + results.Id_access + '" value="4"  onclick="setRecordR(' + results.Id_access + ')" />');
                                }
                                if (results.Record.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Record ' + results.Id_access + '" value="5"  onclick="setRecordR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Record ' + results.Id_access + '" value="5"  onclick="setRecordR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Document Change Request") {
                            if (results.Documents.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="DocChangeReq"  onclick="setDocChangeReq(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="DocChangeReq ' + results.Id_access + '" value="2"  onclick="setDocChangeReqR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="DocChangeReq ' + results.Id_access + '" value="3"  onclick="setDocChangeReqR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="DocChangeReq ' + results.Id_access + '" value="4"  onclick="setDocChangeReqR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="DocChangeReq ' + results.Id_access + '" value="5"  onclick="setDocChangeReqR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.DocChangeReq.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="DocChangeReq"  onclick="setDocChangeReq(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="DocChangeReq"  onclick="setDocChangeReq(' + results.Id_access + ')" />');
                                }
                                if (results.DocChangeReq.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="DocChangeReq ' + results.Id_access + '" value="2"  onclick="setDocChangeReqR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="DocChangeReq ' + results.Id_access + '" value="2"  onclick="setDocChangeReqR(' + results.Id_access + ')" />');
                                }
                                if (results.DocChangeReq.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="DocChangeReq ' + results.Id_access + '" value="3"  onclick="setDocChangeReqR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="DocChangeReq ' + results.Id_access + '" value="3"  onclick="setDocChangeReqR(' + results.Id_access + ')" />');
                                }
                                if (results.DocChangeReq.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="DocChangeReq ' + results.Id_access + '" value="4"  onclick="setDocChangeReqR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="DocChangeReq ' + results.Id_access + '" value="4"  onclick="setDocChangeReqR(' + results.Id_access + ')" />');
                                }
                                if (results.DocChangeReq.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="DocChangeReq ' + results.Id_access + '" value="5"  onclick="setDocChangeReqR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="DocChangeReq ' + results.Id_access + '" value="5"  onclick="setDocChangeReqR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Documents status tracking") {

                            if (results.Documents.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="DocTrack"  onclick="setDocTrack(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="DocTrack ' + results.Id_access + '" value="2"  onclick="setDocTrackR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="DocTrack ' + results.Id_access + '" value="3"  onclick="setDocTrackR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="DocTrack ' + results.Id_access + '" value="4"  onclick="setDocTrackR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="DocTrack ' + results.Id_access + '" value="5"  onclick="setDocTrackR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.DocTrack.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="DocTrack"  onclick="setDocTrack(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="DocTrack"  onclick="setDocTrack(' + results.Id_access + ')" />');
                                }
                                if (results.DocTrack.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="DocTrack ' + results.Id_access + '" value="2"  onclick="setDocTrackR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="DocTrack ' + results.Id_access + '" value="2"  onclick="setDocTrackR(' + results.Id_access + ')" />');
                                }
                                if (results.DocTrack.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="DocTrack ' + results.Id_access + '" value="3"  onclick="setDocTrackR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="DocTrack ' + results.Id_access + '" value="3"  onclick="setDocTrackR(' + results.Id_access + ')" />');
                                }
                                if (results.DocTrack.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="DocTrack ' + results.Id_access + '" value="4"  onclick="setDocTrackR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="DocTrack ' + results.Id_access + '" value="4"  onclick="setDocTrackR(' + results.Id_access + ')" />');
                                }
                                if (results.DocTrack.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="DocTrack ' + results.Id_access + '" value="5"  onclick="setDocTrackR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="DocTrack ' + results.Id_access + '" value="5"  onclick="setDocTrackR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        // --------------End Documents Management------------

                        //----------------Start Objectives/KPIs-------
                        if (node.title == "Objectives/KPIs") {

                            if (results.ObjKPI.includes("1")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="ObjKPI"  onclick="setObjKPI(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="ObjKPI"  onclick="setObjKPI(' + results.Id_access + ')" />');
                            }
                        }
                        if (node.title == "Objectives & targets") {
                            if (results.ObjKPI.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Objectives"  onclick="setObjectives(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Objectives ' + results.Id_access + '" value="2"  onclick="setObjectivesR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Objectives ' + results.Id_access + '" value="3"  onclick="setObjectivesR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Objectives ' + results.Id_access + '" value="4"  onclick="setObjectivesR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Objectives ' + results.Id_access + '" value="5"  onclick="setObjectivesR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Objectives.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Objectives"  onclick="setObjectives(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Objectives"  onclick="setObjectives(' + results.Id_access + ')" />');
                                }
                                if (results.Objectives.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Objectives ' + results.Id_access + '" value="2"  onclick="setObjectivesR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Objectives ' + results.Id_access + '" value="2"  onclick="setObjectivesR(' + results.Id_access + ')" />');
                                }
                                if (results.Objectives.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Objectives ' + results.Id_access + '" value="3"  onclick="setObjectivesR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Objectives ' + results.Id_access + '" value="3"  onclick="setObjectivesR(' + results.Id_access + ')" />');
                                }
                                if (results.Objectives.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Objectives ' + results.Id_access + '" value="4"  onclick="setObjectivesR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Objectives ' + results.Id_access + '" value="4"  onclick="setObjectivesR(' + results.Id_access + ')" />');
                                }
                                if (results.Objectives.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Objectives ' + results.Id_access + '" value="5"  onclick="setObjectivesR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Objectives ' + results.Id_access + '" value="5"  onclick="setObjectivesR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "KPIs") {

                            if (results.ObjKPI.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Kpi"  onclick="setKpi(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Kpi ' + results.Id_access + '" value="2"  onclick="setKpiR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Kpi ' + results.Id_access + '" value="3"  onclick="setKpiR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Kpi ' + results.Id_access + '" value="4"  onclick="setKpiR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Kpi ' + results.Id_access + '" value="5"  onclick="setKpiR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Kpi.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Kpi"  onclick="setKpi(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Kpi"  onclick="setKpi(' + results.Id_access + ')" />');
                                }
                                if (results.Kpi.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Kpi ' + results.Id_access + '" value="2"  onclick="setKpiR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Kpi ' + results.Id_access + '" value="2"  onclick="setKpiR(' + results.Id_access + ')" />');
                                }
                                if (results.Kpi.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Kpi ' + results.Id_access + '" value="3"  onclick="setKpiR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Kpi ' + results.Id_access + '" value="3"  onclick="setKpiR(' + results.Id_access + ')" />');
                                }
                                if (results.Kpi.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Kpi ' + results.Id_access + '" value="4"  onclick="setKpiR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Kpi ' + results.Id_access + '" value="4"  onclick="setKpiR(' + results.Id_access + ')" />');
                                }
                                if (results.Kpi.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Kpi ' + results.Id_access + '" value="5"  onclick="setKpiR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Kpi ' + results.Id_access + '" value="5"  onclick="setKpiR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                         //----------------End Objectives/KPIs-------

                        //----------------Start Context of Organization-------
                        if (node.title == "Context of Organization") {

                            if (results.ContextOrganise.includes("1")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="ContextOrganise"  onclick="setContextOrganise(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="ContextOrganise"  onclick="setContextOrganise(' + results.Id_access + ')" />');
                            }
                        }
                        if (node.title == "Needs & Expectations-Interested Parties") {
                            if (results.ContextOrganise.includes("0")) {
                        		$tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Parties"  onclick="setParties(' + results.Id_access + ')" />');
                        		$tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Parties ' + results.Id_access + '" value="2"  onclick="setPartiesR(' + results.Id_access + ')" />');
                        		$tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Parties ' + results.Id_access + '" value="3"  onclick="setPartiesR(' + results.Id_access + ')" />');
                        		$tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Parties ' + results.Id_access + '" value="4"  onclick="setPartiesR(' + results.Id_access + ')" />');
                        		$tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Parties ' + results.Id_access + '" value="5"  onclick="setPartiesR(' + results.Id_access + ')" />');
                        	}
                        	else {
                        		if (results.Parties.includes("1")) {
                        			$tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Parties"  onclick="setParties(' + results.Id_access + ')" />');
                        		}
                        		else {
                        			$tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Parties"  onclick="setParties(' + results.Id_access + ')" />');
                        		}
                        		if (results.Parties.includes("2")) {
                        			$tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Parties ' + results.Id_access + '" value="2"  onclick="setPartiesR(' + results.Id_access + ')" />');
                        		}
                        		else {
                        			$tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Parties ' + results.Id_access + '" value="2"  onclick="setPartiesR(' + results.Id_access + ')" />');
                        		}
                        		if (results.Parties.includes("3")) {
                        			$tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Parties ' + results.Id_access + '" value="3"  onclick="setPartiesR(' + results.Id_access + ')" />');
                        		}
                        		else {
                        			$tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Parties ' + results.Id_access + '" value="3"  onclick="setPartiesR(' + results.Id_access + ')" />');
                        		}
                        		if (results.Parties.includes("4")) {
                        			$tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Parties ' + results.Id_access + '" value="4"  onclick="setPartiesR(' + results.Id_access + ')" />');
                        		}
                        		else {
                        			$tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Parties ' + results.Id_access + '" value="4"  onclick="setPartiesR(' + results.Id_access + ')" />');
                        		}
                        		if (results.Parties.includes("5")) {
                        			$tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Parties ' + results.Id_access + '" value="5"  onclick="setPartiesR(' + results.Id_access + ')" />');
                        		}
                        		else {
                        			$tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Parties ' + results.Id_access + '" value="5"  onclick="setPartiesR(' + results.Id_access + ')" />');
                        		}
                        	}
                        }
                        if (node.title == "Internal and External Issues") {

                            if (results.ContextOrganise.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Issues"  onclick="setIssues(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Issues ' + results.Id_access + '" value="2"  onclick="setIssuesR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Issues ' + results.Id_access + '" value="3"  onclick="setIssuesR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Issues ' + results.Id_access + '" value="4"  onclick="setIssuesR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Issues ' + results.Id_access + '" value="5"  onclick="setIssuesR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Issues.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Issues"  onclick="setIssues(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Issues"  onclick="setIssues(' + results.Id_access + ')" />');
                                }
                                if (results.Issues.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Issues ' + results.Id_access + '" value="2"  onclick="setIssuesR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Issues ' + results.Id_access + '" value="2"  onclick="setIssuesR(' + results.Id_access + ')" />');
                                }
                                if (results.Issues.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Issues ' + results.Id_access + '" value="3"  onclick="setIssuesR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Issues ' + results.Id_access + '" value="3"  onclick="setIssuesR(' + results.Id_access + ')" />');
                                }
                                if (results.Issues.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Issues ' + results.Id_access + '" value="4"  onclick="setIssuesR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Issues ' + results.Id_access + '" value="4"  onclick="setIssuesR(' + results.Id_access + ')" />');
                                }
                                if (results.Issues.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Issues ' + results.Id_access + '" value="5"  onclick="setIssuesR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Issues ' + results.Id_access + '" value="5"  onclick="setIssuesR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        //----------------End Context of Organization-------

                        //--------------------- Start NC-----------
                        if (node.title == "Nonconformance & Corrective Action") {
                        	if (results.NC.includes("0")) {
                        		$tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="NC"  onclick="setNC(' + results.Id_access + ')" />');
                        		$tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="NC ' + results.Id_access + '" value="2"  onclick="setNCR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="NC ' + results.Id_access + '" value="3"  onclick="setNCR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="NC ' + results.Id_access + '" value="4"  onclick="setNCR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="NC ' + results.Id_access + '" value="5"  onclick="setNCR(' + results.Id_access + ')" />');
                        	}
                        	else {
                                if (results.NC.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="NC"  onclick="setNC(' + results.Id_access + ')" />');
                        		}
                        		else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="NC"  onclick="setNC(' + results.Id_access + ')" />');
                        		}
                                if (results.NC.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="NC ' + results.Id_access + '" value="2"  onclick="setNCR(' + results.Id_access + ')" />');
                        		}
                        		else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="NC ' + results.Id_access + '" value="2"  onclick="setNCR(' + results.Id_access + ')" />');
                        		}
                                if (results.NC.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="NC ' + results.Id_access + '" value="3"  onclick="setNCR(' + results.Id_access + ')" />');
                        		}
                        		else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="NC ' + results.Id_access + '" value="3"  onclick="setNCR(' + results.Id_access + ')" />');
                        		}
                                if (results.NC.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="NC ' + results.Id_access + '" value="4"  onclick="setNCR(' + results.Id_access + ')" />');
                        		}
                        		else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="NC ' + results.Id_access + '" value="4"  onclick="setNCR(' + results.Id_access + ')" />');
                        		}
                        		if (results.NC.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="NC ' + results.Id_access + '" value="5"  onclick="setNCR(' + results.Id_access + ')" />');
                        		}
                        		else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="NC ' + results.Id_access + '" value="5"  onclick="setNCR(' + results.Id_access + ')" />');
                        		}
                        	}
                        }
                        //------------------------------End NC---------------------


                        //----------------Start Portal-------
                        if (node.title == "Portal") {

                            if (results.Portal.includes("1")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Portal"  onclick="setPortal(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Portal"  onclick="setPortal(' + results.Id_access + ')" />');
                            }
                        }
                        if (node.title == "Sub-CR Master") {
                            if (results.Portal.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="sub_cr"  onclick="setSubCr(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="sub_cr ' + results.Id_access + '" value="2"  onclick="setSubCrR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="sub_cr ' + results.Id_access + '" value="3"  onclick="setSubCrR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="sub_cr ' + results.Id_access + '" value="4"  onclick="setSubCrR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="sub_cr ' + results.Id_access + '" value="5"  onclick="setSubCrR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.sub_cr.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="sub_cr"  onclick="setSubCr(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="sub_cr"  onclick="setSubCr(' + results.Id_access + ')" />');
                                }
                                if (results.sub_cr.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="sub_cr ' + results.Id_access + '" value="2"  onclick="setSubCrR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="sub_cr ' + results.Id_access + '" value="2"  onclick="setSubCrR(' + results.Id_access + ')" />');
                                }
                                if (results.sub_cr.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="sub_cr ' + results.Id_access + '" value="3"  onclick="setSubCrR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="sub_cr ' + results.Id_access + '" value="3"  onclick="setSubCrR(' + results.Id_access + ')" />');
                                }
                                if (results.sub_cr.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="sub_cr ' + results.Id_access + '" value="4"  onclick="setSubCrR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="sub_cr ' + results.Id_access + '" value="4"  onclick="setSubCrR(' + results.Id_access + ')" />');
                                }
                                if (results.sub_cr.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="sub_cr ' + results.Id_access + '" value="5"  onclick="setSubCrR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="sub_cr ' + results.Id_access + '" value="5"  onclick="setSubCrR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Access Request-Government") {

                            if (results.Portal.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="access_portal"  onclick="setAccessPortal(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="access_portal ' + results.Id_access + '" value="2"  onclick="setAccessPortalR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="access_portal ' + results.Id_access + '" value="3"  onclick="setAccessPortalR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="access_portal ' + results.Id_access + '" value="4"  onclick="setAccessPortalR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="access_portal ' + results.Id_access + '" value="5"  onclick="setAccessPortalR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.access_portal.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="access_portal"  onclick="setAccessPortal(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="access_portal"  onclick="setAccessPortal(' + results.Id_access + ')" />');
                                }
                                if (results.access_portal.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="access_portal ' + results.Id_access + '" value="2"  onclick="setAccessPortalR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="access_portal ' + results.Id_access + '" value="2"  onclick="setAccessPortalR(' + results.Id_access + ')" />');
                                }
                                if (results.access_portal.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="access_portal ' + results.Id_access + '" value="3"  onclick="setAccessPortalR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="access_portal ' + results.Id_access + '" value="3"  onclick="setAccessPortalR(' + results.Id_access + ')" />');
                                }
                                if (results.access_portal.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="access_portal ' + results.Id_access + '" value="4"  onclick="setAccessPortalR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="access_portal ' + results.Id_access + '" value="4"  onclick="setAccessPortalR(' + results.Id_access + ')" />');
                                }
                                if (results.access_portal.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="access_portal ' + results.Id_access + '" value="5"  onclick="setAccessPortalR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="access_portal ' + results.Id_access + '" value="5"  onclick="setAccessPortalR(' + results.Id_access + ')" />');
                                }
                            }
                        }

                        if (node.title == "Portal Access User Log") {

                            if (results.Portal.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="portal_userlog"  onclick="setPUserLog(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="portal_userlog ' + results.Id_access + '" value="2"  onclick="setPUserLogR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="portal_userlog ' + results.Id_access + '" value="3"  onclick="setPUserLogR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="portal_userlog ' + results.Id_access + '" value="4"  onclick="setPUserLogR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="portal_userlog ' + results.Id_access + '" value="5"  onclick="setPUserLogR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.portal_userlog.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="portal_userlog"  onclick="setPUserLog(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="portal_userlog"  onclick="setPUserLog(' + results.Id_access + ')" />');
                                }
                                if (results.portal_userlog.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="portal_userlog ' + results.Id_access + '" value="2"  onclick="setPUserLogR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="portal_userlog ' + results.Id_access + '" value="2"  onclick="setPUserLogR(' + results.Id_access + ')" />');
                                }
                                if (results.portal_userlog.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="portal_userlog ' + results.Id_access + '" value="3"  onclick="setPUserLogR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="portal_userlog ' + results.Id_access + '" value="3"  onclick="setPUserLogR(' + results.Id_access + ')" />');
                                }
                                if (results.portal_userlog.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="portal_userlog ' + results.Id_access + '" value="4"  onclick="setPUserLogR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="portal_userlog ' + results.Id_access + '" value="4"  onclick="setPUserLogR(' + results.Id_access + ')" />');
                                }
                                if (results.portal_userlog.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="portal_userlog ' + results.Id_access + '" value="5"  onclick="setPUserLogR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="portal_userlog ' + results.Id_access + '" value="5"  onclick="setPUserLogR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        //----------------End Portal-------

                        //-----------------Start Change Mangament--------------- 
                        if (node.title == "Change Management Request") {
                            if (results.ChangeMgmt.includes("0")) {
                        		$tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="ChangeMgmt"  onclick="setChangeMgmt(' + results.Id_access + ')" />');
                        		$tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ChangeMgmt ' + results.Id_access + '" value="2"  onclick="setChangeMgmtR(' + results.Id_access + ')" />');
                        		$tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ChangeMgmt ' + results.Id_access + '" value="3"  onclick="setChangeMgmtR(' + results.Id_access + ')" />');
                        		$tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ChangeMgmt ' + results.Id_access + '" value="4"  onclick="setChangeMgmtR(' + results.Id_access + ')" />');
                        		$tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ChangeMgmt ' + results.Id_access + '" value="5"  onclick="setChangeMgmtR(' + results.Id_access + ')" />');
                        	}
                        	else {
                        		if (results.ChangeMgmt.includes("1")) {
                        			$tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="ChangeMgmt"  onclick="setChangeMgmt(' + results.Id_access + ')" />');
                        		}
                        		else {
                        			$tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="ChangeMgmt"  onclick="setChangeMgmt(' + results.Id_access + ')" />');
                        		}
                        		if (results.ChangeMgmt.includes("2")) {
                        			$tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ChangeMgmt ' + results.Id_access + '" value="2"  onclick="setChangeMgmtR(' + results.Id_access + ')" />');
                        		}
                        		else {
                        			$tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="ChangeMgmt ' + results.Id_access + '" value="2"  onclick="setChangeMgmtR(' + results.Id_access + ')" />');
                        		}
                        		if (results.ChangeMgmt.includes("3")) {
                        			$tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ChangeMgmt ' + results.Id_access + '" value="3"  onclick="setChangeMgmtR(' + results.Id_access + ')" />');
                        		}
                        		else {
                        			$tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="ChangeMgmt ' + results.Id_access + '" value="3"  onclick="setChangeMgmtR(' + results.Id_access + ')" />');
                        		}
                        		if (results.ChangeMgmt.includes("4")) {
                        			$tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ChangeMgmt ' + results.Id_access + '" value="4"  onclick="setChangeMgmtR(' + results.Id_access + ')" />');
                        		}
                        		else {
                        			$tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="ChangeMgmt ' + results.Id_access + '" value="4"  onclick="setChangeMgmtR(' + results.Id_access + ')" />');
                        		}
                        		if (results.ChangeMgmt.includes("5")) {
                        			$tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ChangeMgmt ' + results.Id_access + '" value="5"  onclick="setChangeMgmtR(' + results.Id_access + ')" />');
                        		}
                        		else {
                        			$tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="ChangeMgmt ' + results.Id_access + '" value="5"  onclick="setChangeMgmtR(' + results.Id_access + ')" />');
                        		}
                        	}
                        }
                        //-----------------End Change Mangament--------------- 


                      
                         //------------------Start Risks & Opportunities-----------------------
                        if (node.title == "Risks & Opportunities") {

                            if (results.RiskMgmt.includes("1")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="RiskMgmt"  onclick="setRiskMgmt(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="RiskMgmt"  onclick="setRiskMgmt(' + results.Id_access + ')" />');
                            }

                        }
                        if (node.title == "Qulaity Related Risk") {
                            if (results.RiskMgmt.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Risk"  onclick="setRisk(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Risk ' + results.Id_access + '" value="2"  onclick="setRiskR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Risk ' + results.Id_access + '" value="3"  onclick="setRiskR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Risk ' + results.Id_access + '" value="4"  onclick="setRiskR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Risk ' + results.Id_access + '" value="5"  onclick="setRiskR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Risk.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Risk"  onclick="setRisk(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Risk"  onclick="setRisk(' + results.Id_access + ')" />');
                                }
                                if (results.Risk.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Risk ' + results.Id_access + '" value="2"  onclick="setRiskR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Risk ' + results.Id_access + '" value="2"  onclick="setRiskR(' + results.Id_access + ')" />');
                                }
                                if (results.Risk.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Risk ' + results.Id_access + '" value="3"  onclick="setRiskR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Risk ' + results.Id_access + '" value="3"  onclick="setRiskR(' + results.Id_access + ')" />');
                                }
                                if (results.Risk.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Risk ' + results.Id_access + '" value="4"  onclick="setRiskR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Risk ' + results.Id_access + '" value="4"  onclick="setRiskR(' + results.Id_access + ')" />');
                                }
                                if (results.Risk.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Risk ' + results.Id_access + '" value="5"  onclick="setRiskR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Risk ' + results.Id_access + '" value="5"  onclick="setRiskR(' + results.Id_access + ')" />');
                                }
                            }

                        }
                        if (node.title == "Health&Safety/Envionmental Risks Assesment") {

                            if (results.RiskMgmt.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="HazardRiskReg"  onclick="setHazardRiskReg(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="HazardRiskReg ' + results.Id_access + '" value="2"  onclick="setHazardRiskRegR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="HazardRiskReg ' + results.Id_access + '" value="3"  onclick="setHazardRiskRegR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="HazardRiskReg ' + results.Id_access + '" value="4"  onclick="setHazardRiskRegR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="HazardRiskReg ' + results.Id_access + '" value="5"  onclick="setHazardRiskRegR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.HazardRiskReg.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="HazardRiskReg"  onclick="setHazardRiskReg(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="HazardRiskReg"  onclick="setHazardRiskReg(' + results.Id_access + ')" />');
                                }
                                if (results.HazardRiskReg.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="HazardRiskReg ' + results.Id_access + '" value="2"  onclick="setHazardRiskRegR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="HazardRiskReg ' + results.Id_access + '" value="2"  onclick="setHazardRiskRegR(' + results.Id_access + ')" />');
                                }
                                if (results.HazardRiskReg.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="HazardRiskReg ' + results.Id_access + '" value="3"  onclick="setHazardRiskRegR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="HazardRiskReg ' + results.Id_access + '" value="3"  onclick="setHazardRiskRegR(' + results.Id_access + ')" />');
                                }
                                if (results.HazardRiskReg.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="HazardRiskReg ' + results.Id_access + '" value="4"  onclick="setHazardRiskRegR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="HazardRiskReg ' + results.Id_access + '" value="4"  onclick="setHazardRiskRegR(' + results.Id_access + ')" />');
                                }
                                if (results.HazardRiskReg.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="HazardRiskReg ' + results.Id_access + '" value="5"  onclick="setHazardRiskRegR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="HazardRiskReg ' + results.Id_access + '" value="5"  onclick="setHazardRiskRegR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Job Safety Analysis(JSA)") {

                        	if (results.RiskMgmt.includes("0")) {
                        		$tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="TRA"  onclick="setTRA(' + results.Id_access + ')" />');
                        		$tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="TRA ' + results.Id_access + '" value="2"  onclick="setTRAR(' + results.Id_access + ')" />');
                        		$tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="TRA ' + results.Id_access + '" value="3"  onclick="setTRAR(' + results.Id_access + ')" />');
                        		$tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="TRA ' + results.Id_access + '" value="4"  onclick="setTRAR(' + results.Id_access + ')" />');
                        		$tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="TRA ' + results.Id_access + '" value="5"  onclick="setTRAR(' + results.Id_access + ')" />');
                        	}
                        	else {
                        		if (results.TRA.includes("1")) {
                        			$tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="TRA"  onclick="setTRA(' + results.Id_access + ')" />');
                        		}
                        		else {
                        			$tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="TRA"  onclick="setTRA(' + results.Id_access + ')" />');
                        		}
                        		if (results.TRA.includes("2")) {
                        			$tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="TRA ' + results.Id_access + '" value="2"  onclick="setTRAR(' + results.Id_access + ')" />');
                        		}
                        		else {
                        			$tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="TRA ' + results.Id_access + '" value="2"  onclick="setTRAR(' + results.Id_access + ')" />');
                        		}
                        		if (results.TRA.includes("3")) {
                        			$tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="TRA ' + results.Id_access + '" value="3"  onclick="setTRAR(' + results.Id_access + ')" />');
                        		}
                        		else {
                        			$tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="TRA ' + results.Id_access + '" value="3"  onclick="setTRAR(' + results.Id_access + ')" />');
                        		}
                        		if (results.TRA.includes("4")) {
                        			$tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="TRA ' + results.Id_access + '" value="4"  onclick="setTRAR(' + results.Id_access + ')" />');
                        		}
                        		else {
                        			$tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="TRA ' + results.Id_access + '" value="4"  onclick="setTRAR(' + results.Id_access + ')" />');
                        		}
                        		if (results.TRA.includes("5")) {
                        			$tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="TRA ' + results.Id_access + '" value="5"  onclick="setTRAR(' + results.Id_access + ')" />');
                        		}
                        		else {
                        			$tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="TRA ' + results.Id_access + '" value="5"  onclick="setTRAR(' + results.Id_access + ')" />');
                        		}
                        	}
                        }
                        //------------------End Risks & Opportunities-----------------------

                        //if (node.title == "Resource Consumption List") {

                        //	if (results.RiskMgmt.includes("0")) {
                        //		$tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="ResConsump"  onclick="setResConsump(' + results.Id_access + ')" />');
                        //		$tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ResConsump ' + results.Id_access + '" value="2"  onclick="setResConsumpR(' + results.Id_access + ')" />');
                        //		$tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ResConsump ' + results.Id_access + '" value="3"  onclick="setResConsumpR(' + results.Id_access + ')" />');
                        //		$tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ResConsump ' + results.Id_access + '" value="4"  onclick="setResConsumpR(' + results.Id_access + ')" />');
                        //		$tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ResConsump ' + results.Id_access + '" value="5"  onclick="setResConsumpR(' + results.Id_access + ')" />');
                        //	}
                        //	else {
                        //		if (results.ResConsump.includes("1")) {
                        //			$tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="ResConsump"  onclick="setResConsump(' + results.Id_access + ')" />');
                        //		}
                        //		else {
                        //			$tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="ResConsump"  onclick="setResConsump(' + results.Id_access + ')" />');
                        //		}
                        //		if (results.ResConsump.includes("2")) {
                        //			$tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ResConsump ' + results.Id_access + '" value="2"  onclick="setResConsumpR(' + results.Id_access + ')" />');
                        //		}
                        //		else {
                        //			$tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="ResConsump ' + results.Id_access + '" value="2"  onclick="setResConsumpR(' + results.Id_access + ')" />');
                        //		}
                        //		if (results.ResConsump.includes("3")) {
                        //			$tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ResConsump ' + results.Id_access + '" value="3"  onclick="setResConsumpR(' + results.Id_access + ')" />');
                        //		}
                        //		else {
                        //			$tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="ResConsump ' + results.Id_access + '" value="3"  onclick="setResConsumpR(' + results.Id_access + ')" />');
                        //		}
                        //		if (results.ResConsump.includes("4")) {
                        //			$tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ResConsump ' + results.Id_access + '" value="4"  onclick="setResConsumpR(' + results.Id_access + ')" />');
                        //		}
                        //		else {
                        //			$tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="ResConsump ' + results.Id_access + '" value="4"  onclick="setResConsumpR(' + results.Id_access + ')" />');
                        //		}
                        //		if (results.ResConsump.includes("5")) {
                        //			$tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ResConsump ' + results.Id_access + '" value="5"  onclick="setResConsumpR(' + results.Id_access + ')" />');
                        //		}
                        //		else {
                        //			$tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="ResConsump ' + results.Id_access + '" value="5"  onclick="setResConsumpR(' + results.Id_access + ')" />');
                        //		}
                        //	}
                        //}

                        //---------------------------start HR---------------
                        if (node.title == "Human Resources") {
                            if (results.HR.includes("1")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="HR"  onclick="setHR(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="HR"  onclick="setHR(' + results.Id_access + ')" />');
                            }
                        }
                        if (node.title == "Employee Mgmt") {
                            if (results.HR.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Emp"  onclick="setEmp(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Emp ' + results.Id_access + '" value="2"  onclick="setEmpR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Emp ' + results.Id_access + '" value="3"  onclick="setEmpR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Emp ' + results.Id_access + '" value="4"  onclick="setEmpR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Emp ' + results.Id_access + '" value="5"  onclick="setEmpR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Emp.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Emp"  onclick="setEmp(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Emp"  onclick="setEmp(' + results.Id_access + ')" />');
                                }
                                if (results.Emp.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Emp ' + results.Id_access + '" value="2"  onclick="setEmpR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Emp ' + results.Id_access + '" value="2"  onclick="setEmpR(' + results.Id_access + ')" />');
                                }
                                if (results.Emp.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Emp ' + results.Id_access + '" value="3"  onclick="setEmpR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Emp ' + results.Id_access + '" value="3"  onclick="setEmpR(' + results.Id_access + ')" />');
                                }
                                if (results.Emp.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Emp ' + results.Id_access + '" value="4"  onclick="setEmpR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Emp ' + results.Id_access + '" value="4"  onclick="setEmpR(' + results.Id_access + ')" />');
                                }
                                if (results.Emp.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Emp ' + results.Id_access + '" value="5"  onclick="setEmpR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Emp ' + results.Id_access + '" value="5"  onclick="setEmpR(' + results.Id_access + ')" />');
                                }
                            }

                        }
                        if (node.title == "Employee Performance Evaluation") {
                            if (results.HR.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="EmpPerfEval"  onclick="setEmpPerfEval(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="EmpPerfEval ' + results.Id_access + '" value="2"  onclick="setEmpPerfEvalR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="EmpPerfEval ' + results.Id_access + '" value="3"  onclick="setEmpPerfEvalR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="EmpPerfEval ' + results.Id_access + '" value="4"  onclick="setEmpPerfEvalR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="EmpPerfEval ' + results.Id_access + '" value="5"  onclick="setEmpPerfEvalR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.EmpPerfEval.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="EmpPerfEval"  onclick="setEmpPerfEval(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="EmpPerfEval"  onclick="setEmpPerfEval(' + results.Id_access + ')" />');
                                }
                                if (results.EmpPerfEval.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="EmpPerfEval ' + results.Id_access + '" value="2"  onclick="setEmpPerfEvalR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="EmpPerfEval ' + results.Id_access + '" value="2"  onclick="setEmpPerfEvalR(' + results.Id_access + ')" />');
                                }
                                if (results.EmpPerfEval.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="EmpPerfEval ' + results.Id_access + '" value="3"  onclick="setEmpPerfEvalR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="EmpPerfEval ' + results.Id_access + '" value="3"  onclick="setEmpPerfEvalR(' + results.Id_access + ')" />');
                                }
                                if (results.EmpPerfEval.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="EmpPerfEval ' + results.Id_access + '" value="4"  onclick="setEmpPerfEvalR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="EmpPerfEval ' + results.Id_access + '" value="4"  onclick="setEmpPerfEvalR(' + results.Id_access + ')" />');
                                }
                                if (results.EmpPerfEval.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="EmpPerfEval ' + results.Id_access + '" value="5"  onclick="setEmpPerfEvalR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="EmpPerfEval ' + results.Id_access + '" value="5"  onclick="setEmpPerfEvalR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Competancy Evaluation") {
                            if (results.HR.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Competancy"  onclick="setCompetancy(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Competancy ' + results.Id_access + '" value="2"  onclick="setCompetancyR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Competancy ' + results.Id_access + '" value="3"  onclick="setCompetancyR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Competancy ' + results.Id_access + '" value="4"  onclick="setCompetancyR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Competancy ' + results.Id_access + '" value="5"  onclick="setCompetancyR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Competancy.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Competancy"  onclick="setCompetancy(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Competancy"  onclick="setCompetancy(' + results.Id_access + ')" />');
                                }
                                if (results.Competancy.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Competancy ' + results.Id_access + '" value="2"  onclick="setCompetancyR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Competancy ' + results.Id_access + '" value="2"  onclick="setCompetancyR(' + results.Id_access + ')" />');
                                }
                                if (results.Competancy.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Competancy ' + results.Id_access + '" value="3"  onclick="setCompetancyR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Competancy ' + results.Id_access + '" value="3"  onclick="setCompetancyR(' + results.Id_access + ')" />');
                                }
                                if (results.Competancy.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Competancy ' + results.Id_access + '" value="4"  onclick="setCompetancyR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Competancy ' + results.Id_access + '" value="4"  onclick="setCompetancyR(' + results.Id_access + ')" />');
                                }
                                if (results.Competancy.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Competancy ' + results.Id_access + '" value="5"  onclick="setCompetancyR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Competancy ' + results.Id_access + '" value="5"  onclick="setCompetancyR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Exit Employee List") {
                            if (results.HR.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="ExitEmp"  onclick="setExitEmp(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ExitEmp ' + results.Id_access + '" value="2"  onclick="setExitEmpR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ExitEmp ' + results.Id_access + '" value="3"  onclick="setExitEmpR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ExitEmp ' + results.Id_access + '" value="4"  onclick="setExitEmpR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ExitEmp ' + results.Id_access + '" value="5"  onclick="setExitEmpR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.ExitEmp.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="ExitEmp"  onclick="setExitEmp(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="ExitEmp"  onclick="setExitEmp(' + results.Id_access + ')" />');
                                }
                                if (results.ExitEmp.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ExitEmp ' + results.Id_access + '" value="2"  onclick="setExitEmpR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="ExitEmp ' + results.Id_access + '" value="2"  onclick="setExitEmpR(' + results.Id_access + ')" />');
                                }
                                if (results.ExitEmp.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ExitEmp ' + results.Id_access + '" value="3"  onclick="setExitEmpR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="ExitEmp ' + results.Id_access + '" value="3"  onclick="setExitEmpR(' + results.Id_access + ')" />');
                                }
                                if (results.ExitEmp.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ExitEmp ' + results.Id_access + '" value="4"  onclick="setExitEmpR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="ExitEmp ' + results.Id_access + '" value="4"  onclick="setExitEmpR(' + results.Id_access + ')" />');
                                }
                                if (results.ExitEmp.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ExitEmp ' + results.Id_access + '" value="5"  onclick="setExitEmpR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="ExitEmp ' + results.Id_access + '" value="5"  onclick="setExitEmpR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Visitors") {
                            if (results.HR.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Visitor"  onclick="setVisitor(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Visitor ' + results.Id_access + '" value="2"  onclick="setVisitorR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Visitor ' + results.Id_access + '" value="3"  onclick="setVisitorR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Visitor ' + results.Id_access + '" value="4"  onclick="setVisitorR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Visitor ' + results.Id_access + '" value="5"  onclick="setVisitorR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Visitor.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Visitor"  onclick="setVisitor(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Visitor"  onclick="setVisitor(' + results.Id_access + ')" />');
                                }
                                if (results.Visitor.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Visitor ' + results.Id_access + '" value="2"  onclick="setVisitorR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Visitor ' + results.Id_access + '" value="2"  onclick="setVisitorR(' + results.Id_access + ')" />');
                                }
                                if (results.Visitor.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Visitor ' + results.Id_access + '" value="3"  onclick="setVisitorR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Visitor ' + results.Id_access + '" value="3"  onclick="setVisitorR(' + results.Id_access + ')" />');
                                }
                                if (results.Visitor.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Visitor ' + results.Id_access + '" value="4"  onclick="setVisitorR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Visitor ' + results.Id_access + '" value="4"  onclick="setVisitorR(' + results.Id_access + ')" />');
                                }
                                if (results.Visitor.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Visitor ' + results.Id_access + '" value="5"  onclick="setVisitorR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Visitor ' + results.Id_access + '" value="5"  onclick="setVisitorR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        //---------------------------End HR---------------

                        //---------------------------Start ATS---------------
                        if (node.title == "Actions") {
                            if (results.ATSS.includes("1")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="ATSS"  onclick="setATSS(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="ATSS"  onclick="setATSS(' + results.Id_access + ')" />');
                            }
                        }
                        if (node.title == "Action Tracking Register") {
                            if (results.ATSS.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="ATS"  onclick="setATS(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ATS ' + results.Id_access + '" value="2"  onclick="setATSR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html(' <input type="checkbox" class="form-input-styled" disabled id="select" name="ATS ' + results.Id_access + '" value="3"  onclick="setATSR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ATS ' + results.Id_access + '" value="4"  onclick="setATSR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ATS ' + results.Id_access + '" value="5"  onclick="setATSR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.ATS.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="ATS"  onclick="setATS(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="ATS"  onclick="setATS(' + results.Id_access + ')" />');
                                }
                                if (results.ATS.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ATS ' + results.Id_access + '" value="2"  onclick="setATSR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="ATS ' + results.Id_access + '" value="2"  onclick="setATSR(' + results.Id_access + ')" />');
                                }
                                if (results.ATS.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ATS ' + results.Id_access + '" value="3"  onclick="setATSR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="ATS ' + results.Id_access + '" value="3"  onclick="setATSR(' + results.Id_access + ')" />');
                                }
                                if (results.ATS.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ATS ' + results.Id_access + '" value="4"  onclick="setATSR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="ATS ' + results.Id_access + '" value="4"  onclick="setATSR(' + results.Id_access + ')" />');
                                }
                                if (results.ATS.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ATS ' + results.Id_access + '" value="5"  onclick="setATSR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="ATS ' + results.Id_access + '" value="5"  onclick="setATSR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "HSE Actions Tracking Register") {
                            if (results.ATSS.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="HseATS"  onclick="setHseATS(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="HseATS ' + results.Id_access + '" value="2"  onclick="setHseATSR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="HseATS ' + results.Id_access + '" value="3"  onclick="setHseATSR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="HseATS ' + results.Id_access + '" value="4"  onclick="setHseATSR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="HseATS ' + results.Id_access + '" value="5"  onclick="setHseATSR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.HseATS.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="HseATS"  onclick="setHseATS(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="HseATS"  onclick="setHseATS(' + results.Id_access + ')" />');
                                }
                                if (results.HseATS.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="HseATS ' + results.Id_access + '" value="2"  onclick="setHseATSR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="HseATS ' + results.Id_access + '" value="2"  onclick="setHseATSR(' + results.Id_access + ')" />');
                                }
                                if (results.HseATS.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="HseATS ' + results.Id_access + '" value="3"  onclick="setHseATSR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="HseATS ' + results.Id_access + '" value="3"  onclick="setHseATSR(' + results.Id_access + ')" />');
                                }
                                if (results.HseATS.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="HseATS ' + results.Id_access + '" value="4"  onclick="setHseATSR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="HseATS ' + results.Id_access + '" value="4"  onclick="setHseATSR(' + results.Id_access + ')" />');
                                }
                                if (results.HseATS.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="HseATS ' + results.Id_access + '" value="5"  onclick="setHseATSR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html(' <input type="checkbox" class="form-input-styled" id="select" name="HseATS ' + results.Id_access + '" value="5"  onclick="setHseATSR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        //---------------------------End ATS---------------

                        if (node.title == "Meetings") {
                            if (results.Meeting.includes("1")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Meeting"  onclick="setMeeting(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Meeting"  onclick="setMeeting(' + results.Id_access + ')" />');
                            }
                        }
                        if (node.title == "Meeting Type/Agenda") {
                            if (results.Meeting.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="MAgenda"  onclick="setMAgenda(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="MAgenda ' + results.Id_access + '" value="2"  onclick="setMAgendaR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="MAgenda ' + results.Id_access + '" value="3"  onclick="setMAgendaR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="MAgenda ' + results.Id_access + '" value="4"  onclick="setMAgendaR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="MAgenda ' + results.Id_access + '" value="5"  onclick="setMAgendaR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.MAgenda.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="MAgenda"  onclick="setMAgenda(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="MAgenda"  onclick="setMAgenda(' + results.Id_access + ')" />');
                                }
                                if (results.MAgenda.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="MAgenda ' + results.Id_access + '" value="2"  onclick="setMAgendaR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="MAgenda ' + results.Id_access + '" value="2"  onclick="setMAgendaR(' + results.Id_access + ')" />');
                                }
                                if (results.MAgenda.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="MAgenda ' + results.Id_access + '" value="3"  onclick="setMAgendaR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="MAgenda ' + results.Id_access + '" value="3"  onclick="setMAgendaR(' + results.Id_access + ')" />');
                                }
                                if (results.MAgenda.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="MAgenda ' + results.Id_access + '" value="4"  onclick="setMAgendaR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="MAgenda ' + results.Id_access + '" value="4"  onclick="setMAgendaR(' + results.Id_access + ')" />');
                                }
                                if (results.MAgenda.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="MAgenda ' + results.Id_access + '" value="5"  onclick="setMAgendaR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="MAgenda ' + results.Id_access + '" value="5"  onclick="setMAgendaR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Schedule Meeting") {
                            if (results.Meeting.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="MSchedule"  onclick="setMSchedule(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="MSchedule ' + results.Id_access + '" value="2"  onclick="setMScheduleR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="MSchedule ' + results.Id_access + '" value="3"  onclick="setMScheduleR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="MSchedule ' + results.Id_access + '" value="4"  onclick="setMScheduleR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="MSchedule ' + results.Id_access + '" value="5"  onclick="setMScheduleR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.MSchedule.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="MSchedule"  onclick="setMSchedule(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="MSchedule"  onclick="setMSchedule(' + results.Id_access + ')" />');
                                }
                                if (results.MSchedule.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="MSchedule ' + results.Id_access + '" value="2"  onclick="setMScheduleR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="MSchedule ' + results.Id_access + '" value="2"  onclick="setMScheduleR(' + results.Id_access + ')" />');
                                }
                                if (results.MSchedule.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="MSchedule ' + results.Id_access + '" value="3"  onclick="setMScheduleR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="MSchedule ' + results.Id_access + '" value="3"  onclick="setMScheduleR(' + results.Id_access + ')" />');
                                }
                                if (results.MSchedule.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="MSchedule ' + results.Id_access + '" value="4"  onclick="setMScheduleR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="MSchedule ' + results.Id_access + '" value="4"  onclick="setMScheduleR(' + results.Id_access + ')" />');
                                }
                                if (results.MSchedule.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="MSchedule ' + results.Id_access + '" value="5"  onclick="setMScheduleR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="MSchedule ' + results.Id_access + '" value="5"  onclick="setMScheduleR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Unplanned Meeting") {
                            if (results.Meeting.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="MUnplaned"  onclick="setMUnplaned(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="MUnplaned ' + results.Id_access + '" value="2"  onclick="setMUnplanedR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="MUnplaned ' + results.Id_access + '" value="3"  onclick="setMUnplanedR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="MUnplaned ' + results.Id_access + '" value="4"  onclick="setMUnplanedR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="MUnplaned ' + results.Id_access + '" value="5"  onclick="setMUnplanedR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.MUnplaned.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="MUnplaned"  onclick="setMUnplaned(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="MUnplaned"  onclick="setMUnplaned(' + results.Id_access + ')" />');
                                }
                                if (results.MUnplaned.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="MUnplaned ' + results.Id_access + '" value="2"  onclick="setMUnplanedR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="MUnplaned ' + results.Id_access + '" value="2"  onclick="setMUnplanedR(' + results.Id_access + ')" />');
                                }
                                if (results.MUnplaned.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="MUnplaned ' + results.Id_access + '" value="3"  onclick="setMUnplanedR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="MUnplaned ' + results.Id_access + '" value="3"  onclick="setMUnplanedR(' + results.Id_access + ')" />');
                                }
                                if (results.MUnplaned.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="MUnplaned ' + results.Id_access + '" value="4"  onclick="setMUnplanedR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="MUnplaned ' + results.Id_access + '" value="4"  onclick="setMUnplanedR(' + results.Id_access + ')" />');
                                }
                                if (results.MUnplaned.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="MUnplaned ' + results.Id_access + '" value="5"  onclick="setMUnplanedR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="MUnplaned ' + results.Id_access + '" value="5"  onclick="setMUnplanedR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Suppliers Management") {

                            if (results.Suppliers.includes("1")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Suppliers"  onclick="setSuppliers(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Suppliers"  onclick="setSuppliers(' + results.Id_access + ')" />');
                            }
                        }
                        if (node.title == "Suppliers") {
                            if (results.Suppliers.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Supplier"  onclick="setSupplier(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Supplier ' + results.Id_access + '" value="2"  onclick="setSupplierR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Supplier ' + results.Id_access + '" value="3"  onclick="setSupplierR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Supplier ' + results.Id_access + '" value="4"  onclick="setSupplierR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Supplier ' + results.Id_access + '" value="5"  onclick="setSupplierR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Supplier.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Supplier"  onclick="setSupplier(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Supplier"  onclick="setSupplier(' + results.Id_access + ')" />');
                                }
                                if (results.Supplier.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Supplier ' + results.Id_access + '" value="2"  onclick="setSupplierR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Supplier ' + results.Id_access + '" value="2"  onclick="setSupplierR(' + results.Id_access + ')" />');
                                }
                                if (results.Supplier.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Supplier ' + results.Id_access + '" value="3"  onclick="setSupplierR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Supplier ' + results.Id_access + '" value="3"  onclick="setSupplierR(' + results.Id_access + ')" />');
                                }
                                if (results.Supplier.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Supplier ' + results.Id_access + '" value="4"  onclick="setSupplierR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Supplier ' + results.Id_access + '" value="4"  onclick="setSupplierR(' + results.Id_access + ')" />');
                                }
                                if (results.Supplier.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Supplier ' + results.Id_access + '" value="5"  onclick="setSupplierR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Supplier ' + results.Id_access + '" value="5"  onclick="setSupplierR(' + results.Id_access + ')" />');
                                }
                            }

                        }
                        if (node.title == "Reporting Discrepancies") {
                            if (results.Suppliers.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="DLog"  onclick="setDLog(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="DLog ' + results.Id_access + '" value="2"  onclick="setDLogR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="DLog ' + results.Id_access + '" value="3"  onclick="setDLogR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="DLog ' + results.Id_access + '" value="4"  onclick="setDLogR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="DLog ' + results.Id_access + '" value="5"  onclick="setDLogR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.DLog.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="DLog"  onclick="setDLog(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="DLog"  onclick="setDLog(' + results.Id_access + ')" />');
                                }
                                if (results.DLog.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="DLog ' + results.Id_access + '" value="2"  onclick="setDLogR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="DLog ' + results.Id_access + '" value="2"  onclick="setDLogR(' + results.Id_access + ')" />');
                                }
                                if (results.DLog.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="DLog ' + results.Id_access + '" value="3"  onclick="setDLogR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="DLog ' + results.Id_access + '" value="3"  onclick="setDLogR(' + results.Id_access + ')" />');
                                }
                                if (results.DLog.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="DLog ' + results.Id_access + '" value="4"  onclick="setDLogR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="DLog ' + results.Id_access + '" value="4"  onclick="setDLogR(' + results.Id_access + ')" />');
                                }
                                if (results.DLog.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="DLog ' + results.Id_access + '" value="5"  onclick="setDLogR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="DLog ' + results.Id_access + '" value="5"  onclick="setDLogR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Supplier Performance") {
                            if (results.Suppliers.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="SupplierPer"  onclick="setSupplierPer(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="SupplierPer ' + results.Id_access + '" value="2"  onclick="setSupplierPerR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="SupplierPer ' + results.Id_access + '" value="3"  onclick="setSupplierPerR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="SupplierPer ' + results.Id_access + '" value="4"  onclick="setSupplierPerR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="SupplierPer ' + results.Id_access + '" value="5"  onclick="setSupplierPerR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.SupplierPer.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="SupplierPer"  onclick="setSupplierPer(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="SupplierPer"  onclick="setSupplierPer(' + results.Id_access + ')" />');
                                }
                                if (results.SupplierPer.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="SupplierPer ' + results.Id_access + '" value="2"  onclick="setSupplierPerR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="SupplierPer ' + results.Id_access + '" value="2"  onclick="setSupplierPerR(' + results.Id_access + ')" />');
                                }
                                if (results.SupplierPer.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="SupplierPer ' + results.Id_access + '" value="3"  onclick="setSupplierPerR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="SupplierPer ' + results.Id_access + '" value="3"  onclick="setSupplierPerR(' + results.Id_access + ')" />');
                                }
                                if (results.SupplierPer.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="SupplierPer ' + results.Id_access + '" value="4"  onclick="setSupplierPerR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="SupplierPer ' + results.Id_access + '" value="4"  onclick="setSupplierPerR(' + results.Id_access + ')" />');
                                }
                                if (results.SupplierPer.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="SupplierPer ' + results.Id_access + '" value="5"  onclick="setSupplierPerR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="SupplierPer ' + results.Id_access + '" value="5"  onclick="setSupplierPerR(' + results.Id_access + ')" />');
                                }
                            }

                        }
                        if (node.title == "Approved Ext Provider List") {
                            if (results.Suppliers.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Providers"  onclick="setProviders(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Providers ' + results.Id_access + '" value="2"  onclick="setProvidersR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Providers ' + results.Id_access + '" value="3"  onclick="setProvidersR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Providers ' + results.Id_access + '" value="4"  onclick="setProvidersR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Providers ' + results.Id_access + '" value="5"  onclick="setProvidersR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Providers.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Providers"  onclick="setProviders(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Providers"  onclick="setProviders(' + results.Id_access + ')" />');
                                }
                                if (results.Providers.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Providers ' + results.Id_access + '" value="2"  onclick="setProvidersR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Providers ' + results.Id_access + '" value="2"  onclick="setProvidersR(' + results.Id_access + ')" />');
                                }
                                if (results.Providers.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Providers ' + results.Id_access + '" value="3"  onclick="setProvidersR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Providers ' + results.Id_access + '" value="3"  onclick="setProvidersR(' + results.Id_access + ')" />');
                                }
                                if (results.Providers.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Providers ' + results.Id_access + '" value="4"  onclick="setProvidersR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Providers ' + results.Id_access + '" value="4"  onclick="setProvidersR(' + results.Id_access + ')" />');
                                }
                                if (results.Providers.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Providers ' + results.Id_access + '" value="5"  onclick="setProvidersR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Providers ' + results.Id_access + '" value="5"  onclick="setProvidersR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Performance Evaluation") {
                            if (results.Suppliers.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="SupplierRate"  onclick="setSupplierRate(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="SupplierRate ' + results.Id_access + '" value="2"  onclick="setSupplierRateR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="SupplierRate ' + results.Id_access + '" value="3"  onclick="setSupplierRateR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="SupplierRate ' + results.Id_access + '" value="4"  onclick="setSupplierRateR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="SupplierRate ' + results.Id_access + '" value="5"  onclick="setSupplierRateR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.SupplierRate.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="SupplierRate"  onclick="setSupplierRate(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="SupplierRate"  onclick="setSupplierRate(' + results.Id_access + ')" />');
                                }
                                if (results.SupplierRate.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="SupplierRate ' + results.Id_access + '" value="2"  onclick="setSupplierRateR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="SupplierRate ' + results.Id_access + '" value="2"  onclick="setSupplierRateR(' + results.Id_access + ')" />');
                                }
                                if (results.SupplierRate.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" class="form-input-styled" id="select" checked name="SupplierRate ' + results.Id_access + '" value="3"  onclick="setSupplierRateR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="SupplierRate ' + results.Id_access + '" value="3"  onclick="setSupplierRateR(' + results.Id_access + ')" />');
                                }
                                if (results.SupplierRate.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="SupplierRate ' + results.Id_access + '" value="4"  onclick="setSupplierRateR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="SupplierRate ' + results.Id_access + '" value="4"  onclick="setSupplierRateR(' + results.Id_access + ')" />');
                                }
                                if (results.SupplierRate.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="SupplierRate ' + results.Id_access + '" value="5"  onclick="setSupplierRateR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="SupplierRate ' + results.Id_access + '" value="5"  onclick="setSupplierRateR(' + results.Id_access + ')" />');
                                }
                            }

                        }
                        if (node.title == "Customer") {
                            if (results.CustMgmt.includes("1")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="CustMgmt"  onclick="setCustMgmt(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="CustMgmt"  onclick="setCustMgmt(' + results.Id_access + ')" />');
                            }
                        }
                        if (node.title == "Visits") {
                            if (results.CustMgmt.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Visits"  onclick="setVisits(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Visits ' + results.Id_access + '" value="2"  onclick="setVisitsR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Visits ' + results.Id_access + '" value="3"  onclick="setVisitsR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Visits ' + results.Id_access + '" value="4"  onclick="setVisitsR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Visits ' + results.Id_access + '" value="5"  onclick="setVisitsR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Visits.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Visits"  onclick="setVisits(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Visits"  onclick="setVisits(' + results.Id_access + ')" />');
                                }
                                if (results.Visits.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Visits ' + results.Id_access + '" value="2"  onclick="setVisitsR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Visits ' + results.Id_access + '" value="2"  onclick="setVisitsR(' + results.Id_access + ')" />');
                                }
                                if (results.Visits.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Visits ' + results.Id_access + '" value="3"  onclick="setVisitsR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Visits ' + results.Id_access + '" value="3"  onclick="setVisitsR(' + results.Id_access + ')" />');
                                }
                                if (results.Visits.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Visits ' + results.Id_access + '" value="4"  onclick="setVisitsR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Visits ' + results.Id_access + '" value="4"  onclick="setVisitsR(' + results.Id_access + ')" />');
                                }
                                if (results.Visits.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Visits ' + results.Id_access + '" value="5"  onclick="setVisitsR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Visits ' + results.Id_access + '" value="5"  onclick="setVisitsR(' + results.Id_access + ')" />');
                                }
                            }

                        }
                        if (node.title == "Add Customer/Send Feedback") {

                            if (results.CustMgmt.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="AddCust"  onclick="setAddCust(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AddCust ' + results.Id_access + '" value="2"  onclick="setAddCustR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AddCust ' + results.Id_access + '" value="3"  onclick="setAddCustR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AddCust ' + results.Id_access + '" value="4"  onclick="setAddCustR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AddCust ' + results.Id_access + '" value="5"  onclick="setAddCustR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.AddCust.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="AddCust"  onclick="setAddCust(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="AddCust"  onclick="setAddCust(' + results.Id_access + ')" />');
                                }
                                if (results.AddCust.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AddCust ' + results.Id_access + '" value="2"  onclick="setAddCustR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="AddCust ' + results.Id_access + '" value="2"  onclick="setAddCustR(' + results.Id_access + ')" />');
                                }
                                if (results.AddCust.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AddCust ' + results.Id_access + '" value="3"  onclick="setAddCustR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="AddCust ' + results.Id_access + '" value="3"  onclick="setAddCustR(' + results.Id_access + ')" />');
                                }
                                if (results.AddCust.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AddCust ' + results.Id_access + '" value="4"  onclick="setAddCustR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="AddCust ' + results.Id_access + '" value="4"  onclick="setAddCustR(' + results.Id_access + ')" />');
                                }
                                if (results.AddCust.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AddCust ' + results.Id_access + '" value="5"  onclick="setAddCustR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="AddCust ' + results.Id_access + '" value="5"  onclick="setAddCustR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Complaints") {
                            if (results.CustMgmt.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Complaints"  onclick="setComplaints(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Complaints ' + results.Id_access + '" value="2"  onclick="setComplaintsR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Complaints ' + results.Id_access + '" value="3"  onclick="setComplaintsR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Complaints ' + results.Id_access + '" value="4"  onclick="setComplaintsR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Complaints ' + results.Id_access + '" value="5"  onclick="setComplaintsR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Complaints.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Complaints"  onclick="setComplaints(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Complaints"  onclick="setComplaints(' + results.Id_access + ')" />');
                                }
                                if (results.Complaints.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Complaints ' + results.Id_access + '" value="2"  onclick="setComplaintsR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Complaints ' + results.Id_access + '" value="2"  onclick="setComplaintsR(' + results.Id_access + ')" />');
                                }
                                if (results.Complaints.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Complaints ' + results.Id_access + '" value="3"  onclick="setComplaintsR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Complaints ' + results.Id_access + '" value="3"  onclick="setComplaintsR(' + results.Id_access + ')" />');
                                }
                                if (results.Complaints.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Complaints ' + results.Id_access + '" value="4"  onclick="setComplaintsR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Complaints ' + results.Id_access + '" value="4"  onclick="setComplaintsR(' + results.Id_access + ')" />');
                                }
                                if (results.Complaints.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Complaints ' + results.Id_access + '" value="5"  onclick="setComplaintsR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Complaints ' + results.Id_access + '" value="5"  onclick="setComplaintsR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Upload Survey") {
                            if (results.CustMgmt.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="UploadSurvey"  onclick="setUploadSurvey(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="UploadSurvey ' + results.Id_access + '" value="2"  onclick="setUploadSurveyR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="UploadSurvey ' + results.Id_access + '" value="3"  onclick="setUploadSurveyR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="UploadSurvey ' + results.Id_access + '" value="4"  onclick="setUploadSurveyR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="UploadSurvey ' + results.Id_access + '" value="5"  onclick="setUploadSurveyR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.UploadSurvey.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="UploadSurvey"  onclick="setUploadSurvey(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="UploadSurvey"  onclick="setUploadSurvey(' + results.Id_access + ')" />');
                                }
                                if (results.UploadSurvey.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="UploadSurvey ' + results.Id_access + '" value="2"  onclick="setUploadSurveyR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="UploadSurvey ' + results.Id_access + '" value="2"  onclick="setUploadSurveyR(' + results.Id_access + ')" />');
                                }
                                if (results.UploadSurvey.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="UploadSurvey ' + results.Id_access + '" value="3"  onclick="setUploadSurveyR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="UploadSurvey ' + results.Id_access + '" value="3"  onclick="setUploadSurveyR(' + results.Id_access + ')" />');
                                }
                                if (results.UploadSurvey.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="UploadSurvey ' + results.Id_access + '" value="4"  onclick="setUploadSurveyR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="UploadSurvey ' + results.Id_access + '" value="4"  onclick="setUploadSurveyR(' + results.Id_access + ')" />');
                                }
                                if (results.UploadSurvey.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="UploadSurvey ' + results.Id_access + '" value="5"  onclick="setUploadSurveyR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="UploadSurvey ' + results.Id_access + '" value="5"  onclick="setUploadSurveyR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Customer Property Log") {
                            if (results.CustMgmt.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="CustReturnProcuct"  onclick="setCustReturnProcuct(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="CustReturnProcuct ' + results.Id_access + '" value="2"  onclick="setCustReturnProcuctR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="CustReturnProcuct ' + results.Id_access + '" value="3"  onclick="setCustReturnProcuctR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="CustReturnProcuct ' + results.Id_access + '" value="4"  onclick="setCustReturnProcuctR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="CustReturnProcuct ' + results.Id_access + '" value="5"  onclick="setCustReturnProcuctR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.CustReturnProcuct.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="CustReturnProcuct"  onclick="setCustReturnProcuct(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="CustReturnProcuct"  onclick="setCustReturnProcuct(' + results.Id_access + ')" />');
                                }
                                if (results.CustReturnProcuct.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="CustReturnProcuct ' + results.Id_access + '" value="2"  onclick="setCustReturnProcuctR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="CustReturnProcuct ' + results.Id_access + '" value="2"  onclick="setCustReturnProcuctR(' + results.Id_access + ')" />');
                                }
                                if (results.CustReturnProcuct.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="CustReturnProcuct ' + results.Id_access + '" value="3"  onclick="setCustReturnProcuctR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="CustReturnProcuct ' + results.Id_access + '" value="3"  onclick="setCustReturnProcuctR(' + results.Id_access + ')" />');
                                }
                                if (results.CustReturnProcuct.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="CustReturnProcuct ' + results.Id_access + '" value="4"  onclick="setCustReturnProcuctR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="CustReturnProcuct ' + results.Id_access + '" value="4"  onclick="setCustReturnProcuctR(' + results.Id_access + ')" />');
                                }
                                if (results.CustReturnProcuct.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="CustReturnProcuct ' + results.Id_access + '" value="5"  onclick="setCustReturnProcuctR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="CustReturnProcuct ' + results.Id_access + '" value="5"  onclick="setCustReturnProcuctR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Trainings") {
                            if (results.TrainingOri.includes("1")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="TrainingOri"  onclick="setTrainingOri(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="TrainingOri"  onclick="setTrainingOri(' + results.Id_access + ')" />');
                            }
                        }
                        if (node.title == "Add Training Topic") {

                            if (results.TrainingOri.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="AddTopic"  onclick="setAddTopic(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AddTopic ' + results.Id_access + '" value="2"  onclick="setAddTopicR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AddTopic ' + results.Id_access + '" value="3"  onclick="setAddTopicR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AddTopic ' + results.Id_access + '" value="4"  onclick="setAddTopicR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AddTopic ' + results.Id_access + '" value="5"  onclick="setAddTopicR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.AddTopic.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="AddTopic"  onclick="setAddTopic(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="AddTopic"  onclick="setAddTopic(' + results.Id_access + ')" />');
                                }
                                if (results.AddTopic.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AddTopic ' + results.Id_access + '" value="2"  onclick="setAddTopicR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="AddTopic ' + results.Id_access + '" value="2"  onclick="setAddTopicR(' + results.Id_access + ')" />');
                                }
                                if (results.AddTopic.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AddTopic ' + results.Id_access + '" value="3"  onclick="setAddTopicR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="AddTopic ' + results.Id_access + '" value="3"  onclick="setAddTopicR(' + results.Id_access + ')" />');
                                }
                                if (results.AddTopic.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AddTopic ' + results.Id_access + '" value="4"  onclick="setAddTopicR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="AddTopic ' + results.Id_access + '" value="4"  onclick="setAddTopicR(' + results.Id_access + ')" />');
                                }
                                if (results.AddTopic.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AddTopic ' + results.Id_access + '" value="5"  onclick="setAddTopicR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="AddTopic ' + results.Id_access + '" value="5"  onclick="setAddTopicR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Perform Training") {
                            if (results.TrainingOri.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Perftraining"  onclick="setPerftraining(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Perftraining ' + results.Id_access + '" value="2"  onclick="setPerftrainingR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Perftraining ' + results.Id_access + '" value="3"  onclick="setPerftrainingR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Perftraining ' + results.Id_access + '" value="4"  onclick="setPerftrainingR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Perftraining ' + results.Id_access + '" value="5"  onclick="setPerftrainingR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Perftraining.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Perftraining"  onclick="setPerftraining(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Perftraining"  onclick="setPerftraining(' + results.Id_access + ')" />');
                                }
                                if (results.Perftraining.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Perftraining ' + results.Id_access + '" value="2"  onclick="setPerftrainingR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Perftraining ' + results.Id_access + '" value="2"  onclick="setPerftrainingR(' + results.Id_access + ')" />');
                                }
                                if (results.Perftraining.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Perftraining ' + results.Id_access + '" value="3"  onclick="setPerftrainingR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Perftraining ' + results.Id_access + '" value="3"  onclick="setPerftrainingR(' + results.Id_access + ')" />');
                                }
                                if (results.Perftraining.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Perftraining ' + results.Id_access + '" value="4"  onclick="setPerftrainingR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Perftraining ' + results.Id_access + '" value="4"  onclick="setPerftrainingR(' + results.Id_access + ')" />');
                                }
                                if (results.Perftraining.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Perftraining ' + results.Id_access + '" value="5"  onclick="setPerftrainingR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Perftraining ' + results.Id_access + '" value="5"  onclick="setPerftrainingR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Training Lists") {
                            if (results.TrainingOri.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="EmpTrainingOri"  onclick="setEmpTrainingOri(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="EmpTrainingOri ' + results.Id_access + '" value="2"  onclick="setEmpTrainingOriR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="EmpTrainingOri ' + results.Id_access + '" value="3"  onclick="setEmpTrainingOriR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="EmpTrainingOri ' + results.Id_access + '" value="4"  onclick="setEmpTrainingOriR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="EmpTrainingOri ' + results.Id_access + '" value="5"  onclick="setEmpTrainingOriR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.EmpTrainingOri.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="EmpTrainingOri"  onclick="setEmpTrainingOri(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="EmpTrainingOri"  onclick="setEmpTrainingOri(' + results.Id_access + ')" />');
                                }
                                if (results.EmpTrainingOri.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="EmpTrainingOri ' + results.Id_access + '" value="2"  onclick="setEmpTrainingOriR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="EmpTrainingOri ' + results.Id_access + '" value="2"  onclick="setEmpTrainingOriR(' + results.Id_access + ')" />');
                                }
                                if (results.EmpTrainingOri.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="EmpTrainingOri ' + results.Id_access + '" value="3"  onclick="setEmpTrainingOriR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="EmpTrainingOri ' + results.Id_access + '" value="3"  onclick="setEmpTrainingOriR(' + results.Id_access + ')" />');
                                }
                                if (results.EmpTrainingOri.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="EmpTrainingOri ' + results.Id_access + '" value="4"  onclick="setEmpTrainingOriR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="EmpTrainingOri ' + results.Id_access + '" value="4"  onclick="setEmpTrainingOriR(' + results.Id_access + ')" />');
                                }
                                if (results.EmpTrainingOri.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="EmpTrainingOri ' + results.Id_access + '" value="5"  onclick="setEmpTrainingOriR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="EmpTrainingOri ' + results.Id_access + '" value="5"  onclick="setEmpTrainingOriR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Audits") {
                            if (results.Audit.includes("1")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Audit"  onclick="setAudit(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Audit"  onclick="setAudit(' + results.Id_access + ')" />');
                            }
                        }
                        if (node.title == "Audit Process") {
                            if (results.Audit.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="InterAudit"  onclick="setInterAudit(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="InterAudit ' + results.Id_access + '" value="2"  onclick="setInterAuditR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="InterAudit ' + results.Id_access + '" value="3"  onclick="setInterAuditR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="InterAudit ' + results.Id_access + '" value="4"  onclick="setInterAuditR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="InterAudit ' + results.Id_access + '" value="5"  onclick="setInterAuditR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.InterAudit.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="InterAudit"  onclick="setInterAudit(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="InterAudit"  onclick="setInterAudit(' + results.Id_access + ')" />');
                                }
                                if (results.InterAudit.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="InterAudit ' + results.Id_access + '" value="2"  onclick="setInterAuditR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="InterAudit ' + results.Id_access + '" value="2"  onclick="setInterAuditR(' + results.Id_access + ')" />');
                                }
                                if (results.InterAudit.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="InterAudit ' + results.Id_access + '" value="3"  onclick="setInterAuditR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="InterAudit ' + results.Id_access + '" value="3"  onclick="setInterAuditR(' + results.Id_access + ')" />');
                                }
                                if (results.InterAudit.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="InterAudit ' + results.Id_access + '" value="4"  onclick="setInterAuditR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="InterAudit ' + results.Id_access + '" value="4"  onclick="setInterAuditR(' + results.Id_access + ')" />');
                                }
                                if (results.InterAudit.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="InterAudit ' + results.Id_access + '" value="5"  onclick="setInterAuditR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="InterAudit ' + results.Id_access + '" value="5"  onclick="setInterAuditR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "External Audit") {
                            if (results.Audit.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="ExterAudit"  onclick="setExterAudit(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ExterAudit ' + results.Id_access + '" value="2"  onclick="setExterAuditR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ExterAudit ' + results.Id_access + '" value="3"  onclick="setExterAuditR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ExterAudit ' + results.Id_access + '" value="4"  onclick="setExterAuditR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ExterAudit ' + results.Id_access + '" value="5"  onclick="setExterAuditR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.ExterAudit.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="ExterAudit"  onclick="setExterAudit(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="ExterAudit"  onclick="setExterAudit(' + results.Id_access + ')" />');
                                }
                                if (results.ExterAudit.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ExterAudit ' + results.Id_access + '" value="2"  onclick="setExterAuditR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="ExterAudit ' + results.Id_access + '" value="2"  onclick="setExterAuditR(' + results.Id_access + ')" />');
                                }
                                if (results.ExterAudit.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ExterAudit ' + results.Id_access + '" value="3"  onclick="setExterAuditR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="ExterAudit ' + results.Id_access + '" value="3"  onclick="setExterAuditR(' + results.Id_access + ')" />');
                                }
                                if (results.ExterAudit.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ExterAudit ' + results.Id_access + '" value="4"  onclick="setExterAuditR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="ExterAudit ' + results.Id_access + '" value="4"  onclick="setExterAuditR(' + results.Id_access + ')" />');
                                }
                                if (results.ExterAudit.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ExterAudit ' + results.Id_access + '" value="5"  onclick="setExterAuditR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="ExterAudit ' + results.Id_access + '" value="5"  onclick="setExterAuditR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "External Audit Report") {
                            if (results.Audit.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="ExtAuditRpt"  onclick="setExtAuditRpt(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ExtAuditRpt ' + results.Id_access + '" value="2"  onclick="setExtAuditRptR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ExtAuditRpt ' + results.Id_access + '" value="3"  onclick="setExtAuditRptR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ExtAuditRpt ' + results.Id_access + '" value="4"  onclick="setExtAuditRptR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ExtAuditRpt ' + results.Id_access + '" value="5"  onclick="setExtAuditRptR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.ExtAuditRpt.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="ExtAuditRpt"  onclick="setExtAuditRpt(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="ExtAuditRpt"  onclick="setExtAuditRpt(' + results.Id_access + ')" />');
                                }
                                if (results.ExtAuditRpt.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ExtAuditRpt ' + results.Id_access + '" value="2"  onclick="setExtAuditRptR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="ExtAuditRpt ' + results.Id_access + '" value="2"  onclick="setExtAuditRptR(' + results.Id_access + ')" />');
                                }
                                if (results.ExtAuditRpt.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ExtAuditRpt ' + results.Id_access + '" value="3"  onclick="setExtAuditRptR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="ExtAuditRpt ' + results.Id_access + '" value="3"  onclick="setExtAuditRptR(' + results.Id_access + ')" />');
                                }
                                if (results.ExtAuditRpt.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ExtAuditRpt ' + results.Id_access + '" value="4"  onclick="setExtAuditRptR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="ExtAuditRpt ' + results.Id_access + '" value="4"  onclick="setExtAuditRptR(' + results.Id_access + ')" />');
                                }
                                if (results.ExtAuditRpt.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ExtAuditRpt ' + results.Id_access + '" value="5"  onclick="setExtAuditRptR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="ExtAuditRpt ' + results.Id_access + '" value="5"  onclick="setExtAuditRptR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Customer Audit") {
                            if (results.Audit.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="CustAudit"  onclick="setCustAudit(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="CustAudit ' + results.Id_access + '" value="2"  onclick="setCustAuditR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="CustAudit ' + results.Id_access + '" value="3"  onclick="setCustAuditR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="CustAudit ' + results.Id_access + '" value="4"  onclick="setCustAuditR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="CustAudit ' + results.Id_access + '" value="5"  onclick="setCustAuditR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.CustAudit.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="CustAudit"  onclick="setCustAudit(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="CustAudit"  onclick="setCustAudit(' + results.Id_access + ')" />');
                                }
                                if (results.CustAudit.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="CustAudit ' + results.Id_access + '" value="2"  onclick="setCustAuditR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="CustAudit ' + results.Id_access + '" value="2"  onclick="setCustAuditR(' + results.Id_access + ')" />');
                                }
                                if (results.CustAudit.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="CustAudit ' + results.Id_access + '" value="3"  onclick="setCustAuditR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="CustAudit ' + results.Id_access + '" value="3"  onclick="setCustAuditR(' + results.Id_access + ')" />');
                                }
                                if (results.CustAudit.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="CustAudit ' + results.Id_access + '" value="4"  onclick="setCustAuditR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="CustAudit ' + results.Id_access + '" value="4"  onclick="setCustAuditR(' + results.Id_access + ')" />');
                                }
                                if (results.CustAudit.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="CustAudit ' + results.Id_access + '" value="5"  onclick="setCustAuditR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="CustAudit ' + results.Id_access + '" value="5"  onclick="setCustAuditR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Raise Nc") {
                            if (results.Audit.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="RaiseNc"  onclick="setRaiseNc(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="RaiseNc ' + results.Id_access + '" value="2"  onclick="setRaiseNcR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="RaiseNc ' + results.Id_access + '" value="3"  onclick="setRaiseNcR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="RaiseNc ' + results.Id_access + '" value="4"  onclick="setRaiseNcR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="RaiseNc ' + results.Id_access + '" value="5"  onclick="setRaiseNcR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.RaiseNc.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="RaiseNc"  onclick="setRaiseNc(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="RaiseNc"  onclick="setRaiseNc(' + results.Id_access + ')" />');
                                }
                                if (results.RaiseNc.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="RaiseNc ' + results.Id_access + '" value="2"  onclick="setRaiseNcR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="RaiseNc ' + results.Id_access + '" value="2"  onclick="setRaiseNcR(' + results.Id_access + ')" />');
                                }
                                if (results.RaiseNc.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="RaiseNc ' + results.Id_access + '" value="3"  onclick="setRaiseNcR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="RaiseNc ' + results.Id_access + '" value="3"  onclick="setRaiseNcR(' + results.Id_access + ')" />');
                                }
                                if (results.RaiseNc.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="RaiseNc ' + results.Id_access + '" value="4"  onclick="setRaiseNcR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="RaiseNc ' + results.Id_access + '" value="4"  onclick="setRaiseNcR(' + results.Id_access + ')" />');
                                }
                                if (results.RaiseNc.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="RaiseNc ' + results.Id_access + '" value="5"  onclick="setRaiseNcR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="RaiseNc ' + results.Id_access + '" value="5"  onclick="setRaiseNcR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Internal Audit Checklist(With Upload)") {

                            if (results.Audit.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="InterChecklist"  onclick="setInterChecklist(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="InterChecklist ' + results.Id_access + '" value="2"  onclick="setInterChecklistR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="InterChecklist ' + results.Id_access + '" value="3"  onclick="setInterChecklistR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="InterChecklist ' + results.Id_access + '" value="4"  onclick="setInterChecklistR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="InterChecklist ' + results.Id_access + '" value="5"  onclick="setInterChecklistR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.InterChecklist.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="InterChecklist"  onclick="setInterChecklist(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="InterChecklist"  onclick="setInterChecklist(' + results.Id_access + ')" />');
                                }
                                if (results.InterChecklist.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="InterChecklist ' + results.Id_access + '" value="2"  onclick="setInterChecklistR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="InterChecklist ' + results.Id_access + '" value="2"  onclick="setInterChecklistR(' + results.Id_access + ')" />');
                                }
                                if (results.InterChecklist.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="InterChecklist ' + results.Id_access + '" value="3"  onclick="setInterChecklistR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="InterChecklist ' + results.Id_access + '" value="3"  onclick="setInterChecklistR(' + results.Id_access + ')" />');
                                }
                                if (results.InterChecklist.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="InterChecklist ' + results.Id_access + '" value="4"  onclick="setInterChecklistR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="InterChecklist ' + results.Id_access + '" value="4"  onclick="setInterChecklistR(' + results.Id_access + ')" />');
                                }
                                if (results.InterChecklist.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="InterChecklist ' + results.Id_access + '" value="5"  onclick="setInterChecklistR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="InterChecklist ' + results.Id_access + '" value="5"  onclick="setInterChecklistR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Generate Audit Checklist") {
                            if (results.Audit.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="AuditChecklist"  onclick="setAuditChecklist(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AuditChecklist ' + results.Id_access + '" value="2"  onclick="setAuditChecklistR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AuditChecklist ' + results.Id_access + '" value="3"  onclick="setAuditChecklistR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AuditChecklist ' + results.Id_access + '" value="4"  onclick="setAuditChecklistR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AuditChecklist ' + results.Id_access + '" value="5"  onclick="setAuditChecklistR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.AuditChecklist.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="AuditChecklist"  onclick="setAuditChecklist(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="AuditChecklist"  onclick="setAuditChecklist(' + results.Id_access + ')" />');
                                }
                                if (results.AuditChecklist.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AuditChecklist ' + results.Id_access + '" value="2"  onclick="setAuditChecklistR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="AuditChecklist ' + results.Id_access + '" value="2"  onclick="setAuditChecklistR(' + results.Id_access + ')" />');
                                }
                                if (results.AuditChecklist.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AuditChecklist ' + results.Id_access + '" value="3"  onclick="setAuditChecklistR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="AuditChecklist ' + results.Id_access + '" value="3"  onclick="setAuditChecklistR(' + results.Id_access + ')" />');
                                }
                                if (results.AuditChecklist.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AuditChecklist ' + results.Id_access + '" value="4"  onclick="setAuditChecklistR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="AuditChecklist ' + results.Id_access + '" value="4"  onclick="setAuditChecklistR(' + results.Id_access + ')" />');
                                }
                                if (results.AuditChecklist.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AuditChecklist ' + results.Id_access + '" value="5"  onclick="setAuditChecklistR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="AuditChecklist ' + results.Id_access + '" value="5"  onclick="setAuditChecklistR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "HSE") {
                            if (results.HSE.includes("1")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="HSE"  onclick="setHSE(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="HSE"  onclick="setHSE(' + results.Id_access + ')" />');
                            }
                        }
                        if (node.title == "Incident Reports") {
                            if (results.HSE.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="IncidentRpt"  onclick="setIncidentRpt(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="IncidentRpt ' + results.Id_access + '" value="2"  onclick="setIncidentRptR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="IncidentRpt ' + results.Id_access + '" value="3"  onclick="setIncidentRptR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="IncidentRpt ' + results.Id_access + '" value="4"  onclick="setIncidentRptR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="IncidentRpt ' + results.Id_access + '" value="5"  onclick="setIncidentRptR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.IncidentRpt.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="IncidentRpt"  onclick="setIncidentRpt(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="IncidentRpt"  onclick="setIncidentRpt(' + results.Id_access + ')" />');
                                }
                                if (results.IncidentRpt.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="IncidentRpt ' + results.Id_access + '" value="2"  onclick="setIncidentRptR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="IncidentRpt ' + results.Id_access + '" value="2"  onclick="setIncidentRptR(' + results.Id_access + ')" />');
                                }
                                if (results.IncidentRpt.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="IncidentRpt ' + results.Id_access + '" value="3"  onclick="setIncidentRptR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="IncidentRpt ' + results.Id_access + '" value="3"  onclick="setIncidentRptR(' + results.Id_access + ')" />');
                                }
                                if (results.IncidentRpt.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="IncidentRpt ' + results.Id_access + '" value="4"  onclick="setIncidentRptR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="IncidentRpt ' + results.Id_access + '" value="4"  onclick="setIncidentRptR(' + results.Id_access + ')" />');
                                }
                                if (results.IncidentRpt.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="IncidentRpt ' + results.Id_access + '" value="5"  onclick="setIncidentRptR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="IncidentRpt ' + results.Id_access + '" value="5"  onclick="setIncidentRptR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Near Miss Reporting") {
                            if (results.HSE.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="NearMissRept"  onclick="setNearMissRept(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="NearMissRept ' + results.Id_access + '" value="2"  onclick="setNearMissReptR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="NearMissRept ' + results.Id_access + '" value="3"  onclick="setNearMissReptR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="NearMissRept ' + results.Id_access + '" value="4"  onclick="setNearMissReptR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="NearMissRept ' + results.Id_access + '" value="5"  onclick="setNearMissReptR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.NearMissRept.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="NearMissRept"  onclick="setNearMissRept(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="NearMissRept"  onclick="setNearMissRept(' + results.Id_access + ')" />');
                                }
                                if (results.NearMissRept.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="NearMissRept ' + results.Id_access + '" value="2"  onclick="setNearMissReptR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html(' <input type="checkbox" class="form-input-styled" id="select" name="NearMissRept ' + results.Id_access + '" value="2"  onclick="setNearMissReptR(' + results.Id_access + ')" />');
                                }
                                if (results.NearMissRept.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="NearMissRept ' + results.Id_access + '" value="3"  onclick="setNearMissReptR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="NearMissRept ' + results.Id_access + '" value="3"  onclick="setNearMissReptR(' + results.Id_access + ')" />');
                                }
                                if (results.NearMissRept.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="NearMissRept ' + results.Id_access + '" value="4"  onclick="setNearMissReptR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="NearMissRept ' + results.Id_access + '" value="4"  onclick="setNearMissReptR(' + results.Id_access + ')" />');
                                }
                                if (results.NearMissRept.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="NearMissRept ' + results.Id_access + '" value="5"  onclick="setNearMissReptR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="NearMissRept ' + results.Id_access + '" value="5"  onclick="setNearMissReptR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Emergency Plan And Record") {
                            if (results.HSE.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="EmergPlan"  onclick="setEmergPlan(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="EmergPlan ' + results.Id_access + '" value="2"  onclick="setEmergPlanR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="EmergPlan ' + results.Id_access + '" value="3"  onclick="setEmergPlanR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="EmergPlan ' + results.Id_access + '" value="4"  onclick="setEmergPlanR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="EmergPlan ' + results.Id_access + '" value="5"  onclick="setEmergPlanR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.EmergPlan.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="EmergPlan"  onclick="setEmergPlan(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="EmergPlan"  onclick="setEmergPlan(' + results.Id_access + ')" />');
                                }
                                if (results.EmergPlan.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="EmergPlan ' + results.Id_access + '" value="2"  onclick="setEmergPlanR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="EmergPlan ' + results.Id_access + '" value="2"  onclick="setEmergPlanR(' + results.Id_access + ')" />');
                                }
                                if (results.EmergPlan.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="EmergPlan ' + results.Id_access + '" value="3"  onclick="setEmergPlanR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="EmergPlan ' + results.Id_access + '" value="3"  onclick="setEmergPlanR(' + results.Id_access + ')" />');
                                }
                                if (results.EmergPlan.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="EmergPlan ' + results.Id_access + '" value="4"  onclick="setEmergPlanR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="EmergPlan ' + results.Id_access + '" value="4"  onclick="setEmergPlanR(' + results.Id_access + ')" />');
                                }
                                if (results.EmergPlan.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="EmergPlan ' + results.Id_access + '" value="5"  onclick="setEmergPlanR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="EmergPlan ' + results.Id_access + '" value="5"  onclick="setEmergPlanR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Toolbox Talks Log And Report") {

                            if (results.HSE.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="ToolTalk"  onclick="setToolTalk(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ToolTalk ' + results.Id_access + '" value="2"  onclick="setToolTalkR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ToolTalk ' + results.Id_access + '" value="3"  onclick="setToolTalkR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ToolTalk ' + results.Id_access + '" value="4"  onclick="setToolTalkR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ToolTalk ' + results.Id_access + '" value="5"  onclick="setToolTalkR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.ToolTalk.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="ToolTalk"  onclick="setToolTalk(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="ToolTalk"  onclick="setToolTalk(' + results.Id_access + ')" />');
                                }
                                if (results.ToolTalk.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ToolTalk ' + results.Id_access + '" value="2"  onclick="setToolTalkR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="ToolTalk ' + results.Id_access + '" value="2"  onclick="setToolTalkR(' + results.Id_access + ')" />');
                                }
                                if (results.ToolTalk.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ToolTalk ' + results.Id_access + '" value="3"  onclick="setToolTalkR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="ToolTalk ' + results.Id_access + '" value="3"  onclick="setToolTalkR(' + results.Id_access + ')" />');
                                }
                                if (results.ToolTalk.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ToolTalk ' + results.Id_access + '" value="4"  onclick="setToolTalkR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="ToolTalk ' + results.Id_access + '" value="4"  onclick="setToolTalkR(' + results.Id_access + ')" />');
                                }
                                if (results.ToolTalk.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ToolTalk ' + results.Id_access + '" value="5"  onclick="setToolTalkR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="ToolTalk ' + results.Id_access + '" value="5"  onclick="setToolTalkR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Safety Violation Log And Report") {
                            if (results.HSE.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="SafetyLog"  onclick="setSafetyLog(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="SafetyLog ' + results.Id_access + '" value="2"  onclick="setSafetyLogeR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="SafetyLog ' + results.Id_access + '" value="3"  onclick="setSafetyLogR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="SafetyLog ' + results.Id_access + '" value="4"  onclick="setSafetyLogR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="SafetyLog ' + results.Id_access + '" value="5"  onclick="setSafetyLogR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.SafetyLog.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="SafetyLog"  onclick="setSafetyLog(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="SafetyLog"  onclick="setSafetyLog(' + results.Id_access + ')" />');
                                }
                                if (results.SafetyLog.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="SafetyLog ' + results.Id_access + '" value="2"  onclick="setSafetyLogR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="SafetyLog ' + results.Id_access + '" value="2"  onclick="setSafetyLogR(' + results.Id_access + ')" />');
                                }
                                if (results.SafetyLog.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="SafetyLog ' + results.Id_access + '" value="3"  onclick="setSafetyLogR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="SafetyLog ' + results.Id_access + '" value="3"  onclick="setSafetyLogR(' + results.Id_access + ')" />');
                                }
                                if (results.SafetyLog.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="SafetyLog ' + results.Id_access + '" value="4"  onclick="setSafetyLogR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="SafetyLog ' + results.Id_access + '" value="4"  onclick="setSafetyLogR(' + results.Id_access + ')" />');
                                }
                                if (results.SafetyLog.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="SafetyLog ' + results.Id_access + '" value="5"  onclick="setSafetyLogR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="SafetyLog ' + results.Id_access + '" value="5"  onclick="setSafetyLogR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "PPE Issue Log") {
                            if (results.HSE.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="PpeLog"  onclick="setPpeLog(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="PpeLog ' + results.Id_access + '" value="2"  onclick="setPpeLogR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="PpeLog ' + results.Id_access + '" value="3"  onclick="setPpeLogR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="PpeLog ' + results.Id_access + '" value="4"  onclick="setPpeLogR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="PpeLog ' + results.Id_access + '" value="5"  onclick="setPpeLogR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.PpeLog.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="PpeLog"  onclick="setPpeLog(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="PpeLog"  onclick="setPpeLog(' + results.Id_access + ')" />');
                                }
                                if (results.PpeLog.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="PpeLog ' + results.Id_access + '" value="2"  onclick="setPpeLogR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="PpeLog ' + results.Id_access + '" value="2"  onclick="setPpeLogR(' + results.Id_access + ')" />');
                                }
                                if (results.PpeLog.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="PpeLog ' + results.Id_access + '" value="3"  onclick="setPpeLogR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="PpeLog ' + results.Id_access + '" value="3"  onclick="setPpeLogR(' + results.Id_access + ')" />');
                                }
                                if (results.PpeLog.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="PpeLog ' + results.Id_access + '" value="4"  onclick="setPpeLogR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="PpeLog ' + results.Id_access + '" value="4"  onclick="setPpeLogR(' + results.Id_access + ')" />');
                                }
                                if (results.PpeLog.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="PpeLog ' + results.Id_access + '" value="5"  onclick="setPpeLogR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="PpeLog ' + results.Id_access + '" value="5"  onclick="setPpeLogR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "HSE Inspection") {
                            if (results.HSE.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="HseInsp"  onclick="setHseInsp(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="HseInsp ' + results.Id_access + '" value="2"  onclick="setHseInspR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="HseInsp ' + results.Id_access + '" value="3"  onclick="setHseInspR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="HseInsp ' + results.Id_access + '" value="4"  onclick="setHseInspR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="HseInsp ' + results.Id_access + '" value="5"  onclick="setHseInspR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.HseInsp.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="HseInsp"  onclick="setHseInsp(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="HseInsp"  onclick="setHseInsp(' + results.Id_access + ')" />');
                                }
                                if (results.HseInsp.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="HseInsp ' + results.Id_access + '" value="2"  onclick="setHseInspR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox"  class="form-input-styled" id="select" name="HseInsp ' + results.Id_access + '" value="2"  onclick="setHseInspR(' + results.Id_access + ')" />');
                                }
                                if (results.HseInsp.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="HseInsp ' + results.Id_access + '" value="3"  onclick="setHseInspR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="HseInsp ' + results.Id_access + '" value="3"  onclick="setHseInspR(' + results.Id_access + ')" />');
                                }
                                if (results.HseInsp.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="HseInsp ' + results.Id_access + '" value="4"  onclick="setHseInspR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="HseInsp ' + results.Id_access + '" value="4"  onclick="setHseInspR(' + results.Id_access + ')" />');
                                }
                                if (results.HseInsp.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="HseInsp ' + results.Id_access + '" value="5"  onclick="setHseInspR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="HseInsp ' + results.Id_access + '" value="5"  onclick="setHseInspR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Air/Water/Noise Quality Survey") {
                            if (results.HSE.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="AirNoise"  onclick="setAirNoise(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AirNoise ' + results.Id_access + '" value="2"  onclick="setAirNoiseR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AirNoise ' + results.Id_access + '" value="3"  onclick="setAirNoiseR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AirNoise ' + results.Id_access + '" value="4"  onclick="setAirNoiseR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AirNoise ' + results.Id_access + '" value="5"  onclick="setAirNoiseR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.AirNoise.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="AirNoise"  onclick="setAirNoise(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="AirNoise"  onclick="setAirNoise(' + results.Id_access + ')" />');
                                }
                                if (results.AirNoise.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AirNoise ' + results.Id_access + '" value="2"  onclick="setAirNoiseR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="AirNoise ' + results.Id_access + '" value="2"  onclick="setAirNoiseR(' + results.Id_access + ')" />');
                                }
                                if (results.AirNoise.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AirNoise ' + results.Id_access + '" value="3"  onclick="setAirNoiseR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="AirNoise ' + results.Id_access + '" value="3"  onclick="setAirNoiseR(' + results.Id_access + ')" />');
                                }
                                if (results.AirNoise.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AirNoise ' + results.Id_access + '" value="4"  onclick="setAirNoiseR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="AirNoise ' + results.Id_access + '" value="4"  onclick="setAirNoiseR(' + results.Id_access + ')" />');
                                }
                                if (results.AirNoise.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AirNoise ' + results.Id_access + '" value="5"  onclick="setAirNoiseR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="AirNoise ' + results.Id_access + '" value="5"  onclick="setAirNoiseR(' + results.Id_access + ')" />');
                                }
                            }

                        }
                        if (node.title == "Waste Management") {
                            if (results.HSE.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Waste"  onclick="setWaste(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Waste ' + results.Id_access + '" value="2"  onclick="setWasteR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Waste ' + results.Id_access + '" value="3"  onclick="setWasteR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Waste ' + results.Id_access + '" value="4"  onclick="setWasteR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Waste ' + results.Id_access + '" value="5"  onclick="setWasteR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Waste.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Waste"  onclick="setWaste(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Waste"  onclick="setWaste(' + results.Id_access + ')" />');
                                }
                                if (results.Waste.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Waste ' + results.Id_access + '" value="2"  onclick="setWasteR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Waste ' + results.Id_access + '" value="2"  onclick="setWasteR(' + results.Id_access + ')" />');
                                }
                                if (results.Waste.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Waste ' + results.Id_access + '" value="3"  onclick="setWasteR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Waste ' + results.Id_access + '" value="3"  onclick="setWasteR(' + results.Id_access + ')" />');
                                }
                                if (results.Waste.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Waste ' + results.Id_access + '" value="4"  onclick="setWasteR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Waste ' + results.Id_access + '" value="4"  onclick="setWasteR(' + results.Id_access + ')" />');
                                }
                                if (results.Waste.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Waste ' + results.Id_access + '" value="5"  onclick="setWasteR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Waste ' + results.Id_access + '" value="5"  onclick="setWasteR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Safety Observation Card") {
                            if (results.HSE.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox"  class="form-input-styled"  disabled id="select" name="' + results.Id_access + '" value="ObservCard"  onclick="setObservCard(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ObservCard ' + results.Id_access + '" value="2"  onclick="setObservCardR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ObservCard ' + results.Id_access + '" value="3"  onclick="setObservCardR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ObservCard ' + results.Id_access + '" value="4"  onclick="setObservCardR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ObservCard ' + results.Id_access + '" value="5"  onclick="setObservCardR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.ObservCard.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="ObservCard"  onclick="setObservCard(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="ObservCard"  onclick="setObservCard(' + results.Id_access + ')" />');
                                }
                                if (results.ObservCard.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ObservCard ' + results.Id_access + '" value="2"  onclick="setObservCardR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="ObservCard ' + results.Id_access + '" value="2"  onclick="setObservCardR(' + results.Id_access + ')" />');
                                }
                                if (results.ObservCard.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ObservCard ' + results.Id_access + '" value="3"  onclick="setObservCardR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="ObservCard ' + results.Id_access + '" value="3"  onclick="setObservCardR(' + results.Id_access + ')" />');
                                }
                                if (results.ObservCard.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ObservCard ' + results.Id_access + '" value="4"  onclick="setObservCardR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="ObservCard ' + results.Id_access + '" value="4"  onclick="setObservCardR(' + results.Id_access + ')" />');
                                }
                                if (results.ObservCard.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ObservCard ' + results.Id_access + '" value="5"  onclick="setObservCardR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="ObservCard ' + results.Id_access + '" value="5"  onclick="setObservCardR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "HSE Induction") {
                            if (results.HSE.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="HseIndu"  onclick="setHseIndu(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="HseIndu ' + results.Id_access + '" value="2"  onclick="setHseInduR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="HseIndu ' + results.Id_access + '" value="3"  onclick="setHseInduR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="HseIndu ' + results.Id_access + '" value="4"  onclick="setHseInduR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="HseIndu ' + results.Id_access + '" value="5"  onclick="setHseInduR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.HseIndu.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="HseIndu"  onclick="setHseIndu(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="HseIndu"  onclick="setHseIndu(' + results.Id_access + ')" />');
                                }
                                if (results.HseIndu.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="HseIndu ' + results.Id_access + '" value="2"  onclick="setHseInduR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="HseIndu ' + results.Id_access + '" value="2"  onclick="setHseInduR(' + results.Id_access + ')" />');
                                }
                                if (results.HseIndu.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="HseIndu ' + results.Id_access + '" value="3"  onclick="setHseInduR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="HseIndu ' + results.Id_access + '" value="3"  onclick="setHseInduR(' + results.Id_access + ')" />');
                                }
                                if (results.HseIndu.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="HseIndu ' + results.Id_access + '" value="4"  onclick="setHseInduR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="HseIndu ' + results.Id_access + '" value="4"  onclick="setHseInduR(' + results.Id_access + ')" />');
                                }
                                if (results.HseIndu.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="HseIndu ' + results.Id_access + '" value="5"  onclick="setHseInduR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="HseIndu ' + results.Id_access + '" value="5"  onclick="setHseInduR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "First Aid Box") {
                            if (results.HSE.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="FirstBox"  onclick="setFirstBox(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="FirstBox ' + results.Id_access + '" value="2"  onclick="setFirstBoxR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="FirstBox ' + results.Id_access + '" value="3"  onclick="setFirstBoxR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="FirstBox ' + results.Id_access + '" value="4"  onclick="setFirstBoxR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="FirstBox ' + results.Id_access + '" value="5"  onclick="setFirstBoxR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.FirstBox.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="FirstBox"  onclick="setFirstBox(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="FirstBox"  onclick="setFirstBox(' + results.Id_access + ')" />');
                                }
                                if (results.FirstBox.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="FirstBox ' + results.Id_access + '" value="2"  onclick="setFirstBoxR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="FirstBox ' + results.Id_access + '" value="2"  onclick="setFirstBoxR(' + results.Id_access + ')" />');
                                }
                                if (results.FirstBox.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="FirstBox ' + results.Id_access + '" value="3"  onclick="setFirstBoxR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="FirstBox ' + results.Id_access + '" value="3"  onclick="setFirstBoxR(' + results.Id_access + ')" />');
                                }
                                if (results.FirstBox.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="FirstBox ' + results.Id_access + '" value="4"  onclick="setFirstBoxR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="FirstBox ' + results.Id_access + '" value="4"  onclick="setFirstBoxR(' + results.Id_access + ')" />');
                                }
                                if (results.FirstBox.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="FirstBox ' + results.Id_access + '" value="5"  onclick="setFirstBoxR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="FirstBox ' + results.Id_access + '" value="5"  onclick="setFirstBoxR(' + results.Id_access + ')" />');
                                }
                            }

                        }
                        if (node.title == "Fire Equipment Inspection") {

                            if (results.HSE.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="FireInspection"  onclick="setFireInspection(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="FireInspection ' + results.Id_access + '" value="2"  onclick="setFireInspectionR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="FireInspection ' + results.Id_access + '" value="3"  onclick="setFireInspectionR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="FireInspection ' + results.Id_access + '" value="4"  onclick="setFireInspectionR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="FireInspection ' + results.Id_access + '" value="5"  onclick="setFireInspectionR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.FireInspection.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="FireInspection"  onclick="setFireInspection(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="FireInspection"  onclick="setFireInspection(' + results.Id_access + ')" />');
                                }
                                if (results.FireInspection.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="FireInspection ' + results.Id_access + '" value="2"  onclick="setFireInspectionR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="FireInspection ' + results.Id_access + '" value="2"  onclick="setFireInspectionR(' + results.Id_access + ')" />');
                                }
                                if (results.FireInspection.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="FireInspection ' + results.Id_access + '" value="3"  onclick="setFireInspectionR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="FireInspection ' + results.Id_access + '" value="3"  onclick="setFireInspectionR(' + results.Id_access + ')" />');
                                }
                                if (results.FireInspection.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="FireInspection ' + results.Id_access + '" value="4"  onclick="setFireInspectionR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="FireInspection ' + results.Id_access + '" value="4"  onclick="setFireInspectionR(' + results.Id_access + ')" />');
                                }
                                if (results.FireInspection.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="FireInspection ' + results.Id_access + '" value="5"  onclick="setFireInspectionR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="FireInspection ' + results.Id_access + '" value="5"  onclick="setFireInspectionR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Measuring Equiptments") {
                            if (results.Equip.includes("1")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Equip"  onclick="setEquip(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Equip"  onclick="setEquip(' + results.Id_access + ')" />');
                            }
                        }
                        if (node.title == "Machinery") {
                            if (results.Equip.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Maintenance"  onclick="setMaintenance(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Maintenance ' + results.Id_access + '" value="2"  onclick="setMaintenanceR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Maintenance ' + results.Id_access + '" value="3"  onclick="setMaintenanceR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Maintenance ' + results.Id_access + '" value="4"  onclick="setMaintenanceR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Maintenance ' + results.Id_access + '" value="5"  onclick="setMaintenanceR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Maintenance.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Maintenance"  onclick="setMaintenance(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Maintenance"  onclick="setMaintenance(' + results.Id_access + ')" />');
                                }
                                if (results.Maintenance.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Maintenance ' + results.Id_access + '" value="2"  onclick="setMaintenanceR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Maintenance ' + results.Id_access + '" value="2"  onclick="setMaintenanceR(' + results.Id_access + ')" />');
                                }
                                if (results.Maintenance.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Maintenance ' + results.Id_access + '" value="3"  onclick="setMaintenanceR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Maintenance ' + results.Id_access + '" value="3"  onclick="setMaintenanceR(' + results.Id_access + ')" />');
                                }
                                if (results.Maintenance.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Maintenance ' + results.Id_access + '" value="4"  onclick="setMaintenanceR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Maintenance ' + results.Id_access + '" value="4"  onclick="setMaintenanceR(' + results.Id_access + ')" />');
                                }
                                if (results.Maintenance.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Maintenance ' + results.Id_access + '" value="5"  onclick="setMaintenanceR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Maintenance ' + results.Id_access + '" value="5"  onclick="setMaintenanceR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Tools & Gauges") {
                            if (results.Equip.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Calibration"  onclick="setCalibration(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Calibration ' + results.Id_access + '" value="2"  onclick="setCalibrationR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Calibration ' + results.Id_access + '" value="3"  onclick="setCalibrationR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Calibration ' + results.Id_access + '" value="4"  onclick="setCalibrationR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Calibration ' + results.Id_access + '" value="5"  onclick="setCalibrationR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Calibration.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Calibration"  onclick="setCalibration(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Calibration"  onclick="setCalibration(' + results.Id_access + ')" />');
                                }
                                if (results.Calibration.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Calibration ' + results.Id_access + '" value="2"  onclick="setCalibrationR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Calibration ' + results.Id_access + '" value="2"  onclick="setCalibrationR(' + results.Id_access + ')" />');
                                }
                                if (results.Calibration.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Calibration ' + results.Id_access + '" value="3"  onclick="setCalibrationR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Calibration ' + results.Id_access + '" value="3"  onclick="setCalibrationR(' + results.Id_access + ')" />');
                                }
                                if (results.Calibration.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Calibration ' + results.Id_access + '" value="4"  onclick="setCalibrationR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Calibration ' + results.Id_access + '" value="4"  onclick="setCalibrationR(' + results.Id_access + ')" />');
                                }
                                if (results.Calibration.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Calibration ' + results.Id_access + '" value="5"  onclick="setCalibrationR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Calibration ' + results.Id_access + '" value="5"  onclick="setCalibrationR(' + results.Id_access + ')" />');
                                }
                            }
                        }

                        //-----------------Start Legal Compliance--------------------
                        if (node.title == "Legal Compliance") {
                            if (results.LegalReg.includes("1")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="LegalReg"  onclick="setLegalReg(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="LegalReg"  onclick="setLegalReg(' + results.Id_access + ')" />');
                            }
                        }
                        if (node.title == "Legal Register") {
                            if (results.LegalReg.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Legal"  onclick="setLegal(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" class="form-input-styled" disabled id="select" name="Legal ' + results.Id_access + '" value="2"  onclick="setLegalR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" class="form-input-styled" disabled id="select" name="Legal ' + results.Id_access + '" value="3"  onclick="setLegalR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" class="form-input-styled" disabled id="select" name="Legal ' + results.Id_access + '" value="4"  onclick="setLegalR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" class="form-input-styled" disabled id="select" name="Legal ' + results.Id_access + '" value="5"  onclick="setLegalR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Legal.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Legal"  onclick="setLegal(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Legal"  onclick="setLegal(' + results.Id_access + ')" />');
                                }
                                if (results.Legal.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" class="form-input-styled" id="select" checked name="Legal ' + results.Id_access + '" value="2"  onclick="setLegalR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" class="form-input-styled" id="select" name="Legal ' + results.Id_access + '" value="2"  onclick="setLegalR(' + results.Id_access + ')" />');
                                }
                                if (results.Legal.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" class="form-input-styled" id="select" checked name="Legal ' + results.Id_access + '" value="3"  onclick="setLegalR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" class="form-input-styled" id="select" name="Legal ' + results.Id_access + '" value="3"  onclick="setLegalR(' + results.Id_access + ')" />');
                                }
                                if (results.Legal.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" class="form-input-styled" id="select" checked name="Legal ' + results.Id_access + '" value="4"  onclick="setLegalR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" class="form-input-styled" id="select" name="Legal ' + results.Id_access + '" value="4"  onclick="setLegalR(' + results.Id_access + ')" />');
                                }
                                if (results.Legal.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" class="form-input-styled" id="select" checked name="Legal ' + results.Id_access + '" value="5"  onclick="setLegalR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Legal ' + results.Id_access + '" value="5"  onclick="setLegalR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Certificates") {
                            if (results.LegalReg.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Certificates"  onclick="setCertificates(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Certificates ' + results.Id_access + '" value="2"  onclick="setCertificatesR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Certificates ' + results.Id_access + '" value="3"  onclick="setCertificatesR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Certificates ' + results.Id_access + '" value="4"  onclick="setCertificatesR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Certificates ' + results.Id_access + '" value="5"  onclick="setCertificatesR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Certificates.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Certificates"  onclick="setCertificates(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Certificates"  onclick="setCertificates(' + results.Id_access + ')" />');
                                }
                                if (results.Certificates.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Certificates ' + results.Id_access + '" value="2"  onclick="setCertificatesR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Certificates ' + results.Id_access + '" value="2"  onclick="setCertificatesR(' + results.Id_access + ')" />');
                                }
                                if (results.Certificates.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Certificates ' + results.Id_access + '" value="3"  onclick="setCertificatesR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Certificates ' + results.Id_access + '" value="3"  onclick="setCertificatesR(' + results.Id_access + ')" />');
                                }
                                if (results.Certificates.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Certificates ' + results.Id_access + '" value="4"  onclick="setCertificatesR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Certificates ' + results.Id_access + '" value="4"  onclick="setCertificatesR(' + results.Id_access + ')" />');
                                }
                                if (results.Certificates.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Certificates ' + results.Id_access + '" value="5"  onclick="setCertificatesR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Certificates ' + results.Id_access + '" value="5"  onclick="setCertificatesR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                         //-----------------End Legal Compliance--------------------

                        if (node.title == "Trainings") {
                            if (results.Training.includes("1")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Training"  onclick="setTraining(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Training"  onclick="setTraining(' + results.Id_access + ')" />');
                            }
                        }

                        if (node.title == "TrainingList") {
                            if (results.Training.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="TrainingList"  onclick="setTrainingList(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="TrainingList ' + results.Id_access + '" value="2"  onclick="setTrainingListR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="TrainingList ' + results.Id_access + '" value="3"  onclick="setTrainingListR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="TrainingList ' + results.Id_access + '" value="4"  onclick="setTrainingListR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="TrainingList ' + results.Id_access + '" value="5"  onclick="setTrainingListR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.TrainingList.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="TrainingList"  onclick="setTrainingList(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="TrainingList"  onclick="setTrainingList(' + results.Id_access + ')" />');
                                }
                                if (results.TrainingList.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="TrainingList ' + results.Id_access + '" value="2"  onclick="setTrainingListR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="TrainingList ' + results.Id_access + '" value="2"  onclick="setTrainingListR(' + results.Id_access + ')" />');
                                }
                                if (results.TrainingList.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="TrainingList ' + results.Id_access + '" value="3"  onclick="setTrainingListR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="TrainingList ' + results.Id_access + '" value="3"  onclick="setTrainingListR(' + results.Id_access + ')" />');
                                }
                                if (results.TrainingList.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="TrainingList ' + results.Id_access + '" value="4"  onclick="setTrainingListR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="TrainingList ' + results.Id_access + '" value="4"  onclick="setTrainingListR(' + results.Id_access + ')" />');
                                }
                                if (results.TrainingList.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="TrainingList ' + results.Id_access + '" value="5"  onclick="setTrainingListR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="TrainingList ' + results.Id_access + '" value="5"  onclick="setTrainingListR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Work Permits") {
                            if (results.Permits.includes("1")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Permits"  onclick="setPermits(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Permits"  onclick="setPermits(' + results.Id_access + ')" />');
                            }
                        }
                        if (node.title == "Access Work Permit") {
                            if (results.Permits.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="AccessPermit"  onclick="setAccessPermit(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AccessPermit ' + results.Id_access + '" value="2"  onclick="setAccessPermitR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AccessPermit ' + results.Id_access + '" value="3"  onclick="setAccessPermitR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AccessPermit ' + results.Id_access + '" value="4"  onclick="setAccessPermitR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="AccessPermit ' + results.Id_access + '" value="5"  onclick="setAccessPermitR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.AccessPermit.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="AccessPermit"  onclick="setAccessPermit(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="AccessPermit"  onclick="setAccessPermit(' + results.Id_access + ')" />');
                                }
                                if (results.AccessPermit.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AccessPermit ' + results.Id_access + '" value="2"  onclick="setAccessPermitR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="AccessPermit ' + results.Id_access + '" value="2"  onclick="setAccessPermitR(' + results.Id_access + ')" />');
                                }
                                if (results.AccessPermit.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AccessPermit ' + results.Id_access + '" value="3"  onclick="setAccessPermitR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="AccessPermit ' + results.Id_access + '" value="3"  onclick="setAccessPermitR(' + results.Id_access + ')" />');
                                }
                                if (results.AccessPermit.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AccessPermit ' + results.Id_access + '" value="4"  onclick="setAccessPermitR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="AccessPermit ' + results.Id_access + '" value="4"  onclick="setAccessPermitR(' + results.Id_access + ')" />');
                                }
                                if (results.AccessPermit.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="AccessPermit ' + results.Id_access + '" value="5"  onclick="setAccessPermitR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="AccessPermit ' + results.Id_access + '" value="5"  onclick="setAccessPermitR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Work Permit") {
                            if (results.Permits.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="WorkPermit"  onclick="setWorkPermit(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="WorkPermit ' + results.Id_access + '" value="2"  onclick="setWorkPermitR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="WorkPermit ' + results.Id_access + '" value="3"  onclick="setWorkPermitR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="WorkPermit ' + results.Id_access + '" value="4"  onclick="setWorkPermitR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="WorkPermit ' + results.Id_access + '" value="5"  onclick="setWorkPermitR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.WorkPermit.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="WorkPermit"  onclick="setWorkPermit(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="WorkPermit"  onclick="setWorkPermit(' + results.Id_access + ')" />');
                                }
                                if (results.WorkPermit.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="WorkPermit ' + results.Id_access + '" value="2"  onclick="setWorkPermitR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="WorkPermit ' + results.Id_access + '" value="2"  onclick="setWorkPermitR(' + results.Id_access + ')" />');
                                }
                                if (results.WorkPermit.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="WorkPermit ' + results.Id_access + '" value="3"  onclick="setWorkPermitR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="WorkPermit ' + results.Id_access + '" value="3"  onclick="setWorkPermitR(' + results.Id_access + ')" />');
                                }
                                if (results.WorkPermit.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="WorkPermit ' + results.Id_access + '" value="4"  onclick="setWorkPermitR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="WorkPermit ' + results.Id_access + '" value="4"  onclick="setWorkPermitR(' + results.Id_access + ')" />');
                                }
                                if (results.WorkPermit.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="WorkPermit ' + results.Id_access + '" value="5"  onclick="setWorkPermitR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="WorkPermit ' + results.Id_access + '" value="5"  onclick="setWorkPermitR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Settings") {
                            if (results.Settings.includes("1")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Settings"  onclick="setSettings(' + results.Id_access + ')" />');
                            }
                            else {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Settings"  onclick="setSettings(' + results.Id_access + ')" />');
                            }
                        }
                        if (node.title == "Company Profile") {
                            if (results.Settings.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Company"  onclick="setCompany(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Company ' + results.Id_access + '" value="2"  onclick="setCompanyR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Company ' + results.Id_access + '" value="3"  onclick="setCompanyR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Company ' + results.Id_access + '" value="4"  onclick="setCompanyR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Company ' + results.Id_access + '" value="5"  onclick="setCompanyR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Company.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Company"  onclick="setCompany(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Company"  onclick="setCompany(' + results.Id_access + ')" />');
                                }
                                if (results.Company.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Company ' + results.Id_access + '" value="2"  onclick="setCompanyR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Company ' + results.Id_access + '" value="2"  onclick="setCompanyR(' + results.Id_access + ')" />');
                                }
                                if (results.Company.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Company ' + results.Id_access + '" value="3"  onclick="setCompanyR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Company ' + results.Id_access + '" value="3"  onclick="setCompanyR(' + results.Id_access + ')" />');
                                }
                                if (results.Company.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Company ' + results.Id_access + '" value="4"  onclick="setCompanyR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Company ' + results.Id_access + '" value="4"  onclick="setCompanyR(' + results.Id_access + ')" />');
                                }
                                if (results.Company.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox"  class="form-input-styled" id="select" checked name="Company ' + results.Id_access + '" value="5"  onclick="setCompanyR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Company ' + results.Id_access + '" value="5"  onclick="setCompanyR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Department") {
                            if (results.Settings.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="Dept"  onclick="setDept(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Dept ' + results.Id_access + '" value="2"  onclick="setDeptR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Dept ' + results.Id_access + '" value="3"  onclick="setDeptR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Dept ' + results.Id_access + '" value="4"  onclick="setDeptR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="Dept ' + results.Id_access + '" value="5"  onclick="setDeptR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.Dept.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="Dept"  onclick="setDept(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="Dept"  onclick="setDept(' + results.Id_access + ')" />');
                                }
                                if (results.Dept.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Dept ' + results.Id_access + '" value="2"  onclick="setDeptR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="Dept ' + results.Id_access + '" value="2"  onclick="setDeptR(' + results.Id_access + ')" />');
                                }
                                if (results.Dept.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Dept ' + results.Id_access + '" value="3"  onclick="setDeptR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="Dept ' + results.Id_access + '" value="3"  onclick="setDeptR(' + results.Id_access + ')" />');
                                }
                                if (results.Dept.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Dept ' + results.Id_access + '" value="4"  onclick="setDeptR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="Dept ' + results.Id_access + '" value="4"  onclick="setDeptR(' + results.Id_access + ')" />');
                                }
                                if (results.Dept.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="Dept ' + results.Id_access + '" value="5"  onclick="setDeptR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="Dept ' + results.Id_access + '" value="5"  onclick="setDeptR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "User Profile") {
                            if (results.Settings.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="User"  onclick="setUser(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="User ' + results.Id_access + '" value="2"  onclick="setUserR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="User ' + results.Id_access + '" value="3"  onclick="setUserR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="User ' + results.Id_access + '" value="4"  onclick="setUserR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="User ' + results.Id_access + '" value="5"  onclick="setUserR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.User.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="User"  onclick="setUser(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="User"  onclick="setUser(' + results.Id_access + ')" />');
                                }
                                if (results.User.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="User ' + results.Id_access + '" value="2"  onclick="setUserR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="User ' + results.Id_access + '" value="2"  onclick="setUserR(' + results.Id_access + ')" />');
                                }
                                if (results.User.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="User ' + results.Id_access + '" value="3"  onclick="setUserR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="User ' + results.Id_access + '" value="3"  onclick="setUserR(' + results.Id_access + ')" />');
                                }
                                if (results.User.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="User ' + results.Id_access + '" value="4"  onclick="setUserR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox"  class="form-input-styled" id="select" name="User ' + results.Id_access + '" value="4"  onclick="setUserR(' + results.Id_access + ')" />');
                                }
                                if (results.User.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="User ' + results.Id_access + '" value="5"  onclick="setUserR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="User ' + results.Id_access + '" value="5"  onclick="setUserR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Mgmt Dropdown Data") {
                            if (results.Settings.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="DropDown"  onclick="setDropDown(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="DropDown ' + results.Id_access + '" value="2"  onclick="setDropDownR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="DropDown ' + results.Id_access + '" value="3"  onclick="setDropDownR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="DropDown ' + results.Id_access + '" value="4"  onclick="setDropDownR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="DropDown ' + results.Id_access + '" value="5"  onclick="setDropDownR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.DropDown.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="DropDown"  onclick="setDropDown(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="DropDown"  onclick="setDropDown(' + results.Id_access + ')" />');
                                }
                                if (results.DropDown.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="DropDown ' + results.Id_access + '" value="2"  onclick="setDropDownR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="DropDown ' + results.Id_access + '" value="2"  onclick="setDropDownR(' + results.Id_access + ')" />');
                                }
                                if (results.DropDown.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="DropDown ' + results.Id_access + '" value="3"  onclick="setDropDownR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="DropDown ' + results.Id_access + '" value="3"  onclick="setDropDownR(' + results.Id_access + ')" />');
                                }
                                if (results.DropDown.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="DropDown ' + results.Id_access + '" value="4"  onclick="setDropDownR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="DropDown ' + results.Id_access + '" value="4"  onclick="setDropDownR(' + results.Id_access + ')" />');
                                }
                                if (results.DropDown.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="DropDown ' + results.Id_access + '" value="5"  onclick="setDropDownR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="DropDown ' + results.Id_access + '" value="5"  onclick="setDropDownR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        if (node.title == "Role") {
                            if (results.Settings.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="EmpRole"  onclick="setRole(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="EmpRole ' + results.Id_access + '" value="2"  onclick="setRoleR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="EmpRole ' + results.Id_access + '" value="3"  onclick="setRoleR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="EmpRole ' + results.Id_access + '" value="4"  onclick="setRoleR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="EmpRole ' + results.Id_access + '" value="5"  onclick="setRoleR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.EmpRole.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="EmpRole"  onclick="setRole(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="EmpRole"  onclick="setRole(' + results.Id_access + ')" />');
                                }
                                if (results.EmpRole.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="EmpRole ' + results.Id_access + '" value="2"  onclick="setRoleR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="EmpRole ' + results.Id_access + '" value="2"  onclick="setRoleR(' + results.Id_access + ')" />');
                                }
                                if (results.EmpRole.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="EmpRole ' + results.Id_access + '" value="3"  onclick="setRoleR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="EmpRole ' + results.Id_access + '" value="3"  onclick="setRoleR(' + results.Id_access + ')" />');
                                }
                                if (results.EmpRole.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="EmpRole ' + results.Id_access + '" value="4"  onclick="setRoleR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="EmpRole ' + results.Id_access + '" value="4"  onclick="setRoleR(' + results.Id_access + ')" />');
                                }
                                if (results.EmpRole.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="EmpRole ' + results.Id_access + '" value="5"  onclick="setRoleR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="EmpRole ' + results.Id_access + '" value="5"  onclick="setRoleR(' + results.Id_access + ')" />');
                                }

                            }
                        }
                        if (node.title == "ISO Standards") {
                            if (results.Settings.includes("0")) {
                                $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="' + results.Id_access + '" value="ISOStd"  onclick="setISOStd(' + results.Id_access + ')" />');
                                $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ISOStd ' + results.Id_access + '" value="2"  onclick="setISOStdR(' + results.Id_access + ')" />');
                                $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ISOStd ' + results.Id_access + '" value="3"  onclick="setISOStdR(' + results.Id_access + ')" />');
                                $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ISOStd ' + results.Id_access + '" value="4"  onclick="setISOStdR(' + results.Id_access + ')" />');
                                $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" disabled id="select" name="ISOStd ' + results.Id_access + '" value="5"  onclick="setISOStdR(' + results.Id_access + ')" />');
                            }
                            else {
                                if (results.ISOStd.includes("1")) {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" checked name="' + results.Id_access + '" value="ISOStd"  onclick="setISOStd(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(2).html('<input type="checkbox" class="form-input-styled" id="select" name="' + results.Id_access + '" value="ISOStd"  onclick="setISOStd(' + results.Id_access + ')" />');
                                }
                                if (results.ISOStd.includes("2")) {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ISOStd ' + results.Id_access + '" value="2"  onclick="setISOStdR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(3).html('<input type="checkbox" class="form-input-styled" id="select" name="ISOStd ' + results.Id_access + '" value="2"  onclick="setISOStdR(' + results.Id_access + ')" />');
                                }
                                if (results.ISOStd.includes("3")) {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ISOStd ' + results.Id_access + '" value="3"  onclick="setISOStdR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(4).html('<input type="checkbox" class="form-input-styled" id="select" name="ISOStd ' + results.Id_access + '" value="3"  onclick="setISOStdR(' + results.Id_access + ')" />');
                                }
                                if (results.ISOStd.includes("4")) {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ISOStd ' + results.Id_access + '" value="4"  onclick="setISOStdR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(5).html('<input type="checkbox" class="form-input-styled" id="select" name="ISOStd ' + results.Id_access + '" value="4"  onclick="setISOStdR(' + results.Id_access + ')" />');
                                }
                                if (results.ISOStd.includes("5")) {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" checked name="ISOStd ' + results.Id_access + '" value="5"  onclick="setISOStdR(' + results.Id_access + ')" />');
                                }
                                else {
                                    $tdList.eq(6).html('<input type="checkbox" class="form-input-styled" id="select" name="ISOStd ' + results.Id_access + '" value="5"  onclick="setISOStdR(' + results.Id_access + ')" />');
                                }
                            }
                        }
                        $('.form-input-styled').uniform();
                    }


                });
            }
        });

        if (!$().fancytree) {
            console.warn('Warning - fancytree_all.min.js is not loaded.');
            return;
        }



        // Basic setup
        // ------------------------------

        // Basic example
        $('.tree-default').fancytree({
            init: function (event, data) {
                $('.has-tooltip .fancytree-title').tooltip();
            }
        });

        // Load JSON data
        $('.tree-ajax').fancytree({
            source: {
                url: '../../../../global_assets/demo_data/fancytree/fancytree.json'
            },
            init: function (event, data) {
                $('.has-tooltip .fancytree-title').tooltip();
            }
        });

        // Embed JSON data
        $('.tree-json').fancytree({
            init: function (event, data) {
                $('.has-tooltip .fancytree-title').tooltip();
            }
        });

        // Child counter
        $('.tree-child-count').fancytree({
            extensions: ['childcounter'],
            source: {
                url: '../../../../global_assets/demo_data/fancytree/fancytree.json'
            },
            childcounter: {
                deep: true,
                hideZeros: true,
                hideExpanded: true
            },
            init: function (event, data) {
                $('.has-tooltip .fancytree-title').tooltip();
            }
        });       
    };

    //
    // Return objects assigned to module
    //

    return {
        init: function () {
            _componentFancytree();
        }
    }
}();


// Initialize module
// ------------------------------

document.addEventListener('DOMContentLoaded', function () {

    Fancytree.init();
});
