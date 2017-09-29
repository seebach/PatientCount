
var appid = 'f40fd809-b470-4a69-9764-8c5ea6bb8d8b';

// Must be dynamic and ideally from 

//var apiServer = 'http://appdkba1787/backend';
//var apiServer = 'http://novo7pem/backend';
var apiServer = window.location.protocol + '//' + window.location.host + '/backend';

var qlikServer = 'avzappnlam0022n.corp.novocorp.net';
var app = {};
// array to keep all elements to in saved in
var DBitems = [];
//keeps list of sheets in app
var sheetList = [];
// keeps track of the sheet we have navigated to
var sheetCounter = 0;
// array for selected guid
var selection = [];

$(document).ready(function() {

    var prefix = '/';
    var config = {
        host: qlikServer,
        prefix: prefix,
        port: 443,
        isSecure: true //window.location.protocol === "https:"
    };

    require.config({
        baseUrl: (config.isSecure ? "https://" : "https://") + config.host + (config.port ? ":" + config.port : "") + config.prefix + "resources"
    });

    require(["js/qlik"], function(qlik) {

        $(function() {
            $("#sortable").sortable();
            $("#sortable").disableSelection();
            $("#sortable").on("sortupdate", function(event, ui) {
                // store new sort order
                console.log($('#sortable').sortable('toArray'));

                updateObjects();
                //              console.log($('#sortable').sortable('toArray'));

            });

        });


        var objectCounter = 0;

        // keep all object in app here
        var SheetObjects = [];


        $('#SheetObjectIds').on('change', function() {
            //console.log('Change to ' + this.value);
            app.getObject('qs-sheet-view', this.value);
        })

        function makeGridBox(elementid, qsid) {
            var html = '<div class="col-xs-12 col-sm-6 col-md-6 col-lg-4 qs-box-wrapper qs-objectlist"  data-qsid="' + qsid + '" id="qs-container-' + qsid + '"><div class="qs-box-inner"';
            html += '<div id="' + elementid + '" class="qs-box">' + elementid + '</div>';
            html += '<div class="zoombtn" data-qsid="' + qsid + '" data-toggle="modal" data-target="#modal-zoom" Title="Click for fullsize"></div>';
            html += '<div  class="dragbtn glyphicon glyphicon-move"  Title="Click and drag to change postion"  ></div>';
            html += '<div  class="glyphicon glyphicon-remove removebtn removebtn-ui" data-qsid="' + qsid + '" Title="Remove chart" ></div>';
            html += '</div></div>';
            return html;
        }

        function makeFilterBox(elementid, qsid) {
            var html = '<div class="qs-box-filter qs-objectlist" data-qsid="' + qsid + '" id="qs-container-' + qsid + '"><div>';
            html += '<div id="' + elementid + '" class="">' + elementid + '</div>';
            html += '<div  class="glyphicon glyphicon-remove removebtn removebtn-filter" data-qsid="' + qsid + '"  Title="Remove Filter" ></div>';
            html += '</div></div>';
            return html;
        }

        function insertQlikObj(lastid, qsid, options) {
            log("insert: " + qsid);
            if ( $('#qs-container-'+qsid).length < 1  ){

            $(lastid).before('<div id="placeholder-'+qsid+'" />');
            var elementid = 'QV' + objectCounter;
            var type;

            app.getObjectProperties(qsid).then(function(model) {
                console.log(model.properties.visualization);
                type = model.properties.visualization;
                // handle filters differently
                if (type === 'filterpane') {
                    app.getObject(elementid, qsid, '');
                    var html = makeFilterBox(elementid, qsid);
                    $('#filters').before(html);
                      $('#placeholder-'+qsid).remove();
                } else {
                    app.getObject(elementid, qsid, options);
                    var html = makeGridBox(elementid, qsid);
                    $('#placeholder-'+qsid).replaceWith(html);
                }
            });
            objectCounter++;
        } else {
            log("object already exists: " + qsid);
        }
        }

        $("#insertId").click(function() {
            $.each(selection, (function(index, value) {
                var qsid = value;
                insertQlikObj('#addRow', qsid, {
                    "noInteraction": true,
                    "noSelections": true
                });
            }));
            // find objects that we don't want anymore
            $('.qs-objectlist').each(function() {
                //$(this).data('qsid')
                var id = $(this).data('qsid');
                console.log ($(this).data('qsid'));
                var result = 0;
                $.each(selection, (function(index, value) {
                    console.log(value+' '+id)
                    if(value === id ) { 
                        result = 1; 
                    }
                }));
                // object does not exist so remove it
                if (result === 0) {
                    console.log($(this).data('qsid'));
                    $('#qs-container-' + $(this).data('qsid')).remove();
                }
                //selection.push($(this).data('qsid'));
            });
            // reset the selections array
            $('#qs-sheet-view').html("");
            selection = [];
            //$('#Sheets').find('selected').remove()
            $('#Sheets').prop('selectedIndex', -1);
            setTimeout(function () { updateObjects(); }, 3000);

            

           

        });

        $(document).on('click', ".removebtn", function() {
            $('#qs-container-' + $(this).data('qsid')).remove();
            updateObjects();
        });

       

        function showSheetObjects(sheetGuid) {
            //    var sheetGuid = $("#Sheets option:selected").data("sheetGuid");
            var i = 0;
            var colSize = 100 / 24;
            var rowSize = 100 / 12;

            app.getObject(sheetGuid).then(function(model) {
                model.layout.cells.map(function(d) {
                        console.log("render 1:"+d.name);
                        return {
                            id: d.name,
                            top: d.row * rowSize,
                            left: d.col * colSize,
                            width: d.colspan * colSize,
                            height: d.rowspan * rowSize
                        }
                    })
                    .forEach(function(d) {
                         console.log("render 2:"+d.id);

                        $('#qs-sheet-view').append('<div class="preview-wrapper" id="preview-' + d.id + '" ><span id="select-' + d.id + '" class="glyphicon glyphicon-ok selectedIcon" ></span><div id="show-' + d.id + '" ></div></div>');
                        $('#preview-' + d.id).css({
                            top: 'calc(' + d.top + '%)',
                            left: 'calc(' + d.left + '%)',
                            width: 'calc(' + d.width + '%)',
                            height: 'calc(' + d.height + '%)',
                            position: 'absolute'
                        })

                        app.getObject('show-' + d.id, d.id, {
                            "noInteraction": true,
                            "noSelections": true
                        });
                        $("#preview-" + d.id).click(function() {
                            selectObject($(this).attr('id').replace('preview-', ''))
                            $('#select-' + d.id).toggle();
                            $('#select-' + d.id).toggleClass("selected");
                        });
                        if (selection.indexOf( d.id)!=-1) {
                          $('#select-' + d.id).show();
                          $('#select-' + d.id).addClass("selected");
                        }
                    })
            });
        }

        function selectObject(addGuid) {
            var index = selection.indexOf(addGuid);
            // check if value exists then add or remove it
            if (index > -1) {
                selection.splice(index, 1);
            } else {
                selection.push(addGuid);
            }
            $("#select-" + addGuid).toggle();
            if(selection.length>0) {
              $("#insertId").removeClass('disabled');
            } else {
              $("#insertId").addClass('disabled');
            }
            console.log(selection);
        }



        $(document).on('click', ".zoombtn", function() {
            // console.log($(this).parent()[0].children[0].id);
            var qsid = $(this).data('qsid');
            console.log(qsid);
            app.getObjectProperties(qsid).then(function(model) {
                $('#zoom-modal-title').html(model.properties.title);
            });

            $('#QSZOOM').empty();
            app.getObject('QSZOOM', qsid);
            // attach the sheet to the show details button
            var sheetId = SheetObjects.filter(function( obj ) {
                    if( obj.guid === qsid )
                       { return obj.sheetGuid; }
            }) ;
            $(document).on('click', "#show-details", function() {
                window.open(  (config.isSecure ? "https://" : "https://") + config.host + (config.port ? ":" + config.port : "") + "/sense/app/"+appid +'/sheet/'+sheetId[0].sheetGuid);
            });
         //   qlik.resize();
         //   $('#QSZOOM').show();
         //   $('#QSZOOM').resize();
            //app.visualization.get(qsid).resize();
            app.visualization.get(qsid).then(function(object){
          		object.resize();
          	});
          //  console.log(height);
          //  $('#QSZOOM').height(height + "%");
        });

        function toogleSheetNavigation (state) {
          $("#nextSheet").toggleClass('disabled',state);
          $("#prevSheet").toggleClass('disabled',state);
          $("#qs-sheet-show-overview").toggleClass('disabled',state);
        }

        $(document).on('click', "#addRow", function() {
            // create fresh list of all selected objects 
            selection = [];
            $('.qs-objectlist').each(function() {
                selection.push($(this).data('qsid'));
            });
            $('#modal-content').modal('show');
            $('#qs-sheet-overview').show();
            $("#objectIds").trigger("change");

            toogleSheetNavigation('remove');
        });
        $(document).on('click', ".qs-sheet-preview", function() {
            var sheetGuid = $(this).data('sheet-guid');
            sheetCounter = $(this).data('sheet-number');
              console.log(sheetCounter);
            showSheetObjects(sheetGuid);
            $("#qs-sheet-overview").hide();
            $("#qs-sheet-view").show();
            toogleSheetNavigation('add');
        });
        $(document).on('click', "#qs-sheet-show-overview", function() {
            $("#qs-sheet-overview").show();
            $("#qs-sheet-view").html("");
 
            log(selection);
            toogleSheetNavigation('remove');
            sheetCounter = 0;
        });
            $(document).on('click', "#nextSheet", function() {
            sheetCounter = sheetCounter + 1; // increase i by one
            sheetCounter = sheetCounter % sheetList.length; // if we've gone too high, start from `0` again
            $('#qs-sheet-view').html('');
            console.log(sheetCounter);
            showSheetObjects( sheetList[sheetCounter]); // give us back the item of where we are now
        });

            $(document).on('click', "#prevSheet", function() {
            if (sheetCounter === 0) { // i would become 0
                sheetCounter = sheetList.length; // so put it at the other end of the array
            }
            sheetCounter = sheetCounter - 1; // decrease by one
              $('#qs-sheet-view').html('');
              console.log(sheetCounter);
            showSheetObjects( sheetList[sheetCounter]); // give us back the item of where we are now
        });
        //var app = qlik.openApp('8c01277a-aae5-4f9c-94c7-b02de896fe7e', config);
            var app = qlik.openApp(appid, config);

            app.getObject('filtersRegion', 'bpJdmp');
            app.getObject('filtersCountry', 'SbVqnp');

            var fieldValues = app.field("Country").getData();
            fieldValues.OnData.bind(function () {
                console.info(fieldValues.rows.length);
                if (fieldValues.rows.length <= 1) {
                    $('#filters').hide();
                }
            });

        // Export data from the ExportTable
            var exportObject = app.getObject('0444edc7-9cda-4b40-b02c-d47eb2f3ec03');
            var exportObject2 = app.getObject('sXnpAYd');
            var qTable = qlik.table(exportObject);
            var qTable2 = qlik.table(exportObject2);

            // Must manually create export url as the url must point to the Sense server
            $('#exportData').bind('click', function () {
                // console.log('export clicked');
                qTable.exportData({ download: false, filename: 'DataExportChwI', format: 'OOXML' },
                    function (reply) {
                        // Only https allowed om the Sense server
                        var url = "https://" + qlikServer + reply;
                        // console.log(url);
                        window.open(url);
                    });

                qTable2.exportData({ download: false, filename: 'DataExportOtherData', format: 'OOXML' },
                    function (reply) {
                        // Only https allowed om the Sense server
                        var url = "https://" + qlikServer + reply;
                        // console.log(url);
                        window.open(url);
                    });
            });

        app.getList('sheet', function(reply) {
            count = 0;
            $.each(reply.qAppObjectList.qItems, function(key, sheet) {

                // $("#Sheets").append($('<option></option>').val(sheet.qMeta.title).attr('data-sheet-guid', sheet.qInfo.qId).html(sheet.qMeta.title));
                $("#qs-sheet-overview").append('<div class="col-xs-6 col-sm-6 col-md-4 col-lg-3 qs-sheet-wrapper"><div class="qs-sheet-preview" data-sheet-guid="' + sheet.qInfo.qId + '"  data-sheet-number="' + count + '" >' + sheet.qMeta.title + '<br/><i>' + sheet.qMeta.description + '</i></div></div>');
                sheetList.push(sheet.qInfo.qId);
                $.each(sheet.qData.cells, function(index, value) {
                    if ('type' in value) {
                        SheetObjects.push({
                            'sheet': sheet.qMeta.title,
                            'sheetGuid': sheet.qInfo.qId,
                            'guid': value.name,
                            'type': value.type
                        });
                    }
                });
                count++;
            });
            console.log(SheetObjects);
        });
            $.getJSON( apiServer+"/api/User", function(data) {
                  $.each(JSON.parse(data.QlikObjects),function(index,qid) {
                    console.log(qid);
                    insertQlikObj('#addRow', qid, {
                        "noInteraction": true,
                        "noSelections": true
                    });
                  })
              });

        // navigation buttons
        $("[data-qcmd]").on('click', function() {
            var $element = $(this);
            switch ($element.data('qcmd')) {
                //app level commands
                case 'clearAll':
                    app.clearAll();
                    break;
                case 'back':
                    app.back();
                    break;
                case 'forward':
                    app.forward();
                    break;
                case 'lockAll':
                    app.lockAll();
                    break;
                case 'unlockAll':
                    app.unlockAll();
                    break;
            }
        });

    });

});

function log(message) {
    console.log(message);
}

function updateObjects() {
    DBitems = [];
    $('.qs-objectlist').each(function() {
        DBitems.push($(this).data('qsid'));
    });
    //setLocalStorage();
    setUserApi();
}

function setUserApi() {
  var data = JSON.stringify(DBitems);
  $.post(apiServer+"/api/User",
      {
          QlikObjects: data
      },
      function(data, status){
          log("Data: " + data + "\nStatus: " + status);
      });
}
function setLocalStorage() {
    //saveToStorage.push(DBitems.filter(function(value,index) { return value.guid;}));
    console.log(DBitems);
    localStorage.setItem('defaultObjects', JSON.stringify(DBitems));
}
