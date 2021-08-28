
function getRandomColor() {
    var letters = '0123456789ABCDEF';
    var color = '#';
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}


var EchartsLoader = {
    init: function () {


        $.ajax({
            //url: '/Home/GetSummaryReport',
            url: summary.summaryUrl,
            type: 'POST',
            dataType: "json",
            success: function (result) {                      
                var data = JSON.parse(result);               
                _summary(data.Table);
                _summary2(data.Table9);
                _animatedDonutWithLegend2("#donut_basic_legend", 120, data.Table2);
                _animatedDonutWithLegend2("#donut_basic_legend3", 120, data.Table1);
                _animatedDonutWithLegend2("#donut_basic_legend2", 120, data.Table11);
                _animatedMultipleDonutWithLegend(data.Table6);
                _animatedMultipleDonutWithLegend2(data.Table7);
                var maintenance = _Maintenance(data.Table10);
                var hseInspection=_HseInspection(data.Table8);
                var hseLeading= _HseLeading(data.Table4)
                var hseLagging=_HseLagging(data.Table5)
                var complaintReport = _ComplaintReport(data.Table3);
                if (data.Table13 != null) {
                    _summary3(data.Table13);
                }
                // On sidebar width change
                $(document).on('click', '.sidebar-control', function () {
                    setTimeout(function () {
                        maintenance.resize();
                        hseInspection.resize();
                        hseLeading.resize();
                        hseLagging.resize();
                        complaintReport.resize();
                    }, 0);
                });

                // On window resize
                var resizeCharts;
                window.onresize = function () {
                    clearTimeout(resizeCharts);
                    resizeCharts = setTimeout(function () {
                        
                        maintenance.resize();
                        hseInspection.resize();
                        hseLeading.resize();
                        hseLagging.resize();
                        complaintReport.resize();

                    }, 200);
                };



                $('.nav-tabs a').click(function () {
                  
                    $(window).trigger('resize');

                })
            },
            error: function (data) {
                console.log(data);
            }
        });

     //------------------function summary 


        var _summary = function (data) {
            
            document.getElementById("empcnt").innerHTML = data[0].total;
            document.getElementById("auditcnt").innerHTML = data[1].total;
            document.getElementById("nccnt").innerHTML = data[2].total;
            document.getElementById("riskcnt").innerHTML = data[3].total;
            document.getElementById("doccnt").innerHTML = data[4].total;
            document.getElementById("custcnt").innerHTML = data[5].total;
            document.getElementById("supcnt").innerHTML = data[6].total;
            document.getElementById("Evesupcnt").innerHTML = data[7].total;
           
        }




        //------------------function summary2 


        var _summary2 = function (data) {

            document.getElementById("extauditcnt").innerHTML = data[0].total;
            document.getElementById("incidentcnt").innerHTML = data[1].total;
            document.getElementById("interauditcnt").innerHTML = data[2].total;
            document.getElementById("meetingcnt").innerHTML = data[3].total;
            document.getElementById("nearmisscnt").innerHTML = data[4].total;
          
        }

        //------------------function summary2 


        var _summary3 = function (data) {

            document.getElementById("qc1").innerHTML = data[0].total;
            document.getElementById("qc2").innerHTML = data[1].total;
            document.getElementById("qc3").innerHTML = data[2].total;
            document.getElementById("qc4").innerHTML = data[3].total;
            document.getElementById("qc5").innerHTML = data[4].total;

        }
       
         //-----------------donut widge creation


         // Donut with legend
        var _animatedDonutWithLegend2 = function (element, size,data) {
            if (typeof d3 == 'undefined') {
                console.warn('Warning - d3.min.js is not loaded.');
                return;
            }

           

                  


                 

                    var doctype = []; //month
                    var total = [];

                    $.each(data, function (key, item) {

                        doctype.push(item.doctype);
                        total.push(item.total);

                    })


                    // Initialize chart only if element exsists in the DOM
                    if (element) {
                      
                        // Add data set



                        var c = doctype.length;
                        //var data = [
                        //    {
                        //        "status": "New",
                        //        "value": 790,
                        //        "color": "#29B6F6"
                        //    }, {
                        //        "status": "Pending",
                        //        "value": 850,
                        //        "color": "#66BB6A"
                        //    }, {
                        //        "status": "Shipped",
                        //        "value": 760,
                        //        "color": "#EF5350"
                        //    }
                        //];
                        var data = [];

                        for (var i = 0; i < c; i++) {

                            data.push({
                                status: doctype[i],
                                value: total[i],
                                color: getRandomColor()
                            });
                        }
                      

                        // Main variables
                        var d3Container = d3.select(element),
                            distance = 2, // reserve 2px space for mouseover arc moving
                            radius = (size / 2) - distance,
                            sum = d3.sum(data, function (d) { return d.value; });


                        // Create chart
                        // ------------------------------

                        // Add svg element
                        var container = d3Container.append("svg");

                        // Add SVG group
                        var svg = container
                            .attr("width", size)
                            .attr("height", size)
                            .append("g")
                            .attr("transform", "translate(" + (size / 2) + "," + (size / 2) + ")");


                        // Construct chart layout
                        // ------------------------------

                        // Pie
                        var pie = d3.layout.pie()
                            .sort(null)
                            .startAngle(Math.PI)
                            .endAngle(3 * Math.PI)
                            .value(function (d) {
                                return d.value;
                            });

                        // Arc
                        var arc = d3.svg.arc()
                            .outerRadius(radius)
                            .innerRadius(radius / 1.5);


                        //
                        // Append chart elements
                        //

                        // Group chart elements
                        var arcGroup = svg.selectAll(".d3-arc")
                            .data(pie(data))
                            .enter()
                            .append("g")
                            .attr("class", "d3-arc")
                            .style({
                                'stroke': '#fff',
                                'stroke-width': 2,
                                'cursor': 'pointer'
                            });

                        // Append path
                        var arcPath = arcGroup
                            .append("path")
                            .style("fill", function (d) {
                                return d.data.color;
                            });


                        // Add interactions
                        arcPath
                            .on('mouseover', function (d, i) {

                                // Transition on mouseover
                                d3.select(this)
                                    .transition()
                                    .duration(500)
                                    .ease('elastic')
                                    .attr('transform', function (d) {
                                        d.midAngle = ((d.endAngle - d.startAngle) / 2) + d.startAngle;
                                        var x = Math.sin(d.midAngle) * distance;
                                        var y = -Math.cos(d.midAngle) * distance;
                                        return 'translate(' + x + ',' + y + ')';
                                    });

                                // Animate legend
                                $(element + ' [data-slice]').css({
                                    'opacity': 0.3,
                                    'transition': 'all ease-in-out 0.15s'
                                });
                                $(element + ' [data-slice=' + i + ']').css({ 'opacity': 1 });
                            })
                            .on('mouseout', function (d, i) {

                                // Mouseout transition
                                d3.select(this)
                                    .transition()
                                    .duration(500)
                                    .ease('bounce')
                                    .attr('transform', 'translate(0,0)');

                                // Revert legend animation
                                $(element + ' [data-slice]').css('opacity', 1);
                            });

                        // Animate chart on load
                        arcPath
                            .transition()
                            .delay(function (d, i) {
                                return i * 500;
                            })
                            .duration(500)
                            .attrTween("d", function (d) {
                                var interpolate = d3.interpolate(d.startAngle, d.endAngle);
                                return function (t) {
                                    d.endAngle = interpolate(t);
                                    return arc(d);
                                };
                            });


                        //
                        // Append counter
                        //

                        // Append text
                        svg
                            .append('text')
                            .attr('text-anchor', 'middle')
                            .attr('dy', 6)
                            .style({
                                'font-size': '17px',
                                'font-weight': 500
                            });

                        // Animate text
                        svg.select('text')
                            .transition()
                            .duration(1500)
                            .tween("text", function (d) {
                                var i = d3.interpolate(this.textContent, sum);
                                return function (t) {
                                    this.textContent = d3.format(",d")(Math.round(i(t)));
                                };
                            });


                        //
                        // Append legend
                        //

                        // Add element
                        var legend = d3.select(element)
                            .append('ul')
                            .attr('class', 'chart-widget-legend')
                            .selectAll('li').data(pie(data))
                            .enter().append('li')
                            .attr('data-slice', function (d, i) {
                                return i;
                            })
                            .attr('style', function (d, i) {
                                return 'border-bottom: 2px solid ' + d.data.color;
                            })
                            .text(function (d, i) {
                                return d.data.status + ': ';
                            });

                        // Add value
                        legend.append('span')
                            .text(function (d, i) {
                                return d.data.value;
                            });
                    }
               
            
        };

      
        //-----------------donut widge creation


        // Donut with legend
        var _MultipleDonutWithLegend = function (element, size, data) {
            if (typeof d3 == 'undefined') {
                console.warn('Warning - d3.min.js is not loaded.');
                return;
            }

            // Initialize chart only if element exsists in the DOM
            if (element) {

                // Add data set
               

                // Main variables
                var d3Container = d3.select(element),
                    distance = 2, // reserve 2px space for mouseover arc moving
                    radius = (size / 2) - distance,
                    sum = d3.sum(data, function (d) { return d.value; });


                // Create chart
                // ------------------------------

                // Add svg element
                var container = d3Container.append("svg");

                // Add SVG group
                var svg = container
                    .attr("width", size)
                    .attr("height", size)
                    .append("g")
                    .attr("transform", "translate(" + (size / 2) + "," + (size / 2) + ")");


                // Construct chart layout
                // ------------------------------

                // Pie
                var pie = d3.layout.pie()
                    .sort(null)
                    .startAngle(Math.PI)
                    .endAngle(3 * Math.PI)
                    .value(function (d) {
                        return d.value;
                    });

                // Arc
                var arc = d3.svg.arc()
                    .outerRadius(radius)
                    .innerRadius(radius / 1.5);


                //
                // Append chart elements
                //

                // Group chart elements
                var arcGroup = svg.selectAll(".d3-arc")
                    .data(pie(data))
                    .enter()
                    .append("g")
                    .attr("class", "d3-arc")
                    .style({
                        'stroke': '#fff',
                        'stroke-width': 2,
                        'cursor': 'pointer'
                    });

                // Append path
                var arcPath = arcGroup
                    .append("path")
                    .style("fill", function (d) {
                        return d.data.color;
                    });


                // Add interactions
                arcPath
                    .on('mouseover', function (d, i) {

                        // Transition on mouseover
                        d3.select(this)
                            .transition()
                            .duration(500)
                            .ease('elastic')
                            .attr('transform', function (d) {
                                d.midAngle = ((d.endAngle - d.startAngle) / 2) + d.startAngle;
                                var x = Math.sin(d.midAngle) * distance;
                                var y = -Math.cos(d.midAngle) * distance;
                                return 'translate(' + x + ',' + y + ')';
                            });

                        // Animate legend
                        $(element + ' [data-slice]').css({
                            'opacity': 0.3,
                            'transition': 'all ease-in-out 0.15s'
                        });
                        $(element + ' [data-slice=' + i + ']').css({ 'opacity': 1 });
                    })
                    .on('mouseout', function (d, i) {

                        // Mouseout transition
                        d3.select(this)
                            .transition()
                            .duration(500)
                            .ease('bounce')
                            .attr('transform', 'translate(0,0)');

                        // Revert legend animation
                        $(element + ' [data-slice]').css('opacity', 1);
                    });

                // Animate chart on load
                arcPath
                    .transition()
                    .delay(function (d, i) {
                        return i * 500;
                    })
                    .duration(500)
                    .attrTween("d", function (d) {
                        var interpolate = d3.interpolate(d.startAngle, d.endAngle);
                        return function (t) {
                            d.endAngle = interpolate(t);
                            return arc(d);
                        };
                    });


                //
                // Append counter
                //

                // Append text
                svg
                    .append('text')
                    .attr('text-anchor', 'middle')
                    .attr('dy', 6)
                    .style({
                        'font-size': '17px',
                        'font-weight': 500
                    });

                // Animate text
                svg.select('text')
                    .transition()
                    .duration(1500)
                    .tween("text", function (d) {
                        var i = d3.interpolate(this.textContent, sum);
                        return function (t) {
                            this.textContent = d3.format(",d")(Math.round(i(t)));
                        };
                    });


                //
                // Append legend
                //

                // Add element
                var legend = d3.select(element)
                    .append('ul')
                    .attr('class', 'chart-widget-legend')
                    .selectAll('li').data(pie(data))
                    .enter().append('li')
                    .attr('data-slice', function (d, i) {
                        return i;
                    })
                    .attr('style', function (d, i) {
                        return 'border-bottom: 2px solid ' + d.data.color;
                    })
                    .text(function (d, i) {
                        return d.data.status + ': ';
                    });

                // Add value
                legend.append('span')
                    .text(function (d, i) {
                        return d.data.value;
                    });
            }

        };

        //------------------function ComplaintReport


        var _ComplaintReport = function (data) {
            var complchart_element = document.getElementById('complchart');
            if (complchart_element) {

                // Initialize chart
                var complchart = echarts.init(complchart_element);

                var month = []; //month
                var total = [];

                $.each(data, function (key, item) {

                    month.push(item.MonthName);
                    total.push(item.No_complaints);

                });

                complchart.setOption({

                    // Define colors
                    color: [
                        '#7eb00a', '#b6a2df', '#5ab1ef', '#ffb980', '#d87a80',
                        '#8d98b3', '#e5cf0d', '#97b552', '#95706d', '#dc69aa',
                        '#07a2a4', '#9a7fd1', '#588dd5', '#f5994e', '#c05050',
                        '#59678c', '#c9ab00', '#7eb00a', '#6f5553', '#c14089'
                    ],

                    // Global text styles
                    textStyle: {
                        fontFamily: 'Roboto, Arial, Verdana, sans-serif',
                        fontSize: 13
                    },

                    // Chart animation duration
                    animationDuration: 750,

                    // Setup grid
                    grid: {
                        left: 0,
                        right: 40,
                        top: 35,
                        bottom: 0,
                        containLabel: true
                    },

                    // Add legend
                    legend: {
                        show: false,
                        data: month,
                        itemHeight: 8,
                        itemGap: 20,
                        textStyle: {
                            padding: [0, 5]
                        }
                    },

                    // Add tooltip
                    tooltip: {
                        trigger: 'axis',
                        backgroundColor: 'rgba(0,0,0,0.75)',
                        padding: [10, 15],
                        textStyle: {
                            fontSize: 13,
                            fontFamily: 'Roboto, sans-serif'
                        }

                    },

                    // Horizontal axis
                    xAxis: [{
                        type: 'category',
                        data: month,
                        axisLabel: {
                            color: '#333'
                        },
                        axisLine: {
                            lineStyle: {
                                color: '#999'
                            }
                        },
                        splitLine: {
                            show: true,
                            lineStyle: {
                                color: '#eee',
                                type: 'dashed'
                            }
                        }
                    }],

                    // Vertical axis
                    yAxis: [{
                        type: 'value',
                        axisLabel: {
                            color: '#333'
                        },
                        axisLine: {
                            lineStyle: {
                                color: '#999'
                            }
                        },
                        splitLine: {
                            lineStyle: {
                                color: ['#eee']
                            }
                        },
                        splitArea: {
                            show: true,
                            areaStyle: {
                                color: ['rgba(250,250,250,0.1)', 'rgba(0,0,0,0.01)']
                            }
                        }
                    }],

                    // Add series
                    series: [
                        {
                            //name: doctype,
                            type: 'bar',
                            data: total,
                            itemStyle: {
                                normal: {
                                    label: {
                                        show: true,
                                        position: 'top',
                                        textStyle: {
                                            fontWeight: 500
                                        }
                                    }
                                }
                            }

                            //markLine: {
                            //    data: [{ type: 'average', name: 'Average' }]
                            //}
                        }

                    ]
                });
            }
            return complchart;
        }

      
        //------------------function HseLagging 


        var _HseLeading = function (data) {

            var hseleading_element = document.getElementById('hseleading');

            var ind_desc = [];
            var total = [];

            $.each(data, function (key, item) {

                ind_desc.push(item.ind_desc);
                total.push(item.total);

            });
            var dataobj = [];
            var dataseries = [];

            $.each(data, function (key, item) {

                dataobj.push(
                    {
                        value: item.total,
                        name: item.ind_desc
                    }
                )

            });
            $.each(dataobj, function (key, item) {

                dataseries.push({ value: item.value, name: item.name })


            })

            if (hseleading_element) {

                var hseldchart = echarts.init(hseleading_element);
                // Options
                hseldchart.setOption({

                    // Colors
                    color: [
                        '#95706d', '#c05050', '#9a7fd1', '#f5994e', '#d87a80',
                        '#8d98b3', '#e5cf0d', '#97b552', '#95706d', '#dc69aa',
                        '#07a2a4', '#9a7fd1', '#588dd5', '#f5994e', '#c05050',
                        '#59678c', '#c9ab00', '#7eb00a', '#6f5553', '#c14089'
                    ],

                    // Global text styles
                    textStyle: {
                        fontFamily: 'Roboto, Arial, Verdana, sans-serif',
                        fontSize: 13
                    },

                    // Add title
                    title: {
                        //text: 'Browser popularity',
                        //subtext: 'Open source information',
                        left: 'center',
                        textStyle: {
                            fontSize: 17,
                            fontWeight: 500
                        },
                        subtextStyle: {
                            fontSize: 12
                        }
                    },

                    // Add tooltip
                    tooltip: {
                        trigger: 'item',
                        backgroundColor: 'rgba(0,0,0,0.75)',
                        padding: [10, 15],
                        textStyle: {
                            fontSize: 13,
                            fontFamily: 'Roboto, sans-serif'
                        },
                        formatter: "{a} <br/>{b}: {c}"
                    },

                    // Add legend
                    legend: {
                        show: false,
                        orient: 'vertical',
                        top: 'center',
                        left: 0,
                        data: ind_desc,
                        itemHeight: 8,
                        itemWidth: 8
                    },

                    // Add series
                    series: [{
                        name: 'Leading Indicator',
                        type: 'pie',
                        radius: ['50%', '70%'],
                        center: ['50%', '57.5%'],
                        itemStyle: {
                            normal: {
                                borderWidth: 1,
                                borderColor: '#fff'
                            }
                        },
                        data: dataseries
                    }]
                });

            }
            return hseldchart;
   
        }







       

        //------------------function HseLagging 


        var _HseLagging = function (data) {
            var hselagging_element = document.getElementById('hselagging');
            var ind_desc = [];
            var total = [];

            $.each(data, function (key, item) {

                ind_desc.push(item.ind_desc);
                total.push(item.total);

            });
            var dataobj = [];
            var dataseries = [];

            $.each(data, function (key, item) {

                dataobj.push(
                    {
                        value: item.total,
                        name: item.ind_desc
                    }
                )

            });
            $.each(dataobj, function (key, item) {

                dataseries.push({ value: item.value, name: item.name })


            })

            if (hselagging_element) {

                var hselgchart = echarts.init(hselagging_element);
                // Options
                hselgchart.setOption({

                    // Colors
                    color: [
                        '#95706d', '#c05050', '#9a7fd1', '#f5994e', '#d87a80',
                        '#8d98b3', '#e5cf0d', '#97b552', '#95706d', '#dc69aa',
                        '#07a2a4', '#9a7fd1', '#588dd5', '#f5994e', '#c05050',
                        '#59678c', '#c9ab00', '#7eb00a', '#6f5553', '#c14089'
                    ],

                    // Global text styles
                    textStyle: {
                        fontFamily: 'Roboto, Arial, Verdana, sans-serif',
                        fontSize: 13
                    },

                    // Add title
                    title: {
                        //text: 'Browser popularity',
                        //subtext: 'Open source information',
                        left: 'center',
                        textStyle: {
                            fontSize: 17,
                            fontWeight: 500
                        },
                        subtextStyle: {
                            fontSize: 12
                        }
                    },

                    // Add tooltip
                    tooltip: {
                        trigger: 'item',
                        backgroundColor: 'rgba(0,0,0,0.75)',
                        padding: [10, 15],
                        textStyle: {
                            fontSize: 13,
                            fontFamily: 'Roboto, sans-serif'
                        },
                        formatter: "{a} <br/>{b}: {c}"
                    },

                    // Add legend
                    legend: {
                        show: false,
                        orient: 'vertical',
                        top: 'center',
                        left: 0,
                        data: ind_desc,
                        itemHeight: 8,
                        itemWidth: 8
                    },

                    // Add series
                    series: [{
                        name: 'Lagging Indicator',
                        type: 'pie',
                        radius: ['50%', '70%'],
                        center: ['50%', '57.5%'],
                        itemStyle: {
                            normal: {
                                borderWidth: 1,
                                borderColor: '#fff'
                            }
                        },
                        data: dataseries
                    }]
                });

            }
          

            return hselgchart;
      
        }


       




        //-----------------donut widge creation


        // Donut with legend
        var _animatedMultipleDonutWithLegend = function (data) {
            if (typeof d3 == 'undefined') {
                console.warn('Warning - d3.min.js is not loaded.');
                return;
            } 
            
                    var dept = [];
                    var achieved = [];
                    var in_progress = [];
                    var Not_achieved = [];
                    var Established = [];

                    $.each(data, function (key, item) {


                        dept.push(item.deptname);
                        achieved.push(item.achieved);
                        in_progress.push(item.in_progress);
                        Not_achieved.push(item.Not_achieved);
                        Established.push(item.Established);

                    });

                  
                  

                   
                  

                        // Add data set



                        var c = dept.length;
                       
                        var data = [];
                        var cnt = 1;
                        for (var i = 0; i < c; i++) {
                            data = [];
                            data.push({
                                status: "Achieved",
                                value: achieved[i],
                                color: getRandomColor()
                            });

                            data.push({
                                status: "In Progress",
                                value: in_progress[i],
                                color: getRandomColor()
                            });


                            data.push({
                                status: "Not Achieved",
                                value: Not_achieved[i],
                                color: getRandomColor()
                            });


                            data.push({
                                status: "Established",
                                value: Established[i],
                                color: getRandomColor()
                            });
                           

                         
                            if (cnt == 1) {

                                var lineb = document.createElement('hr')
                                var header = document.createElement('div')
                                header.setAttribute('class', 'mb-3');
                                var headingmain = document.createElement('h6')
                                headingmain.setAttribute('class', 'font-weight-semibold mb-0 mt-1');
                                headingmain.textContent = "Objectives By Department";
                                header.appendChild(headingmain)
                                var zz = document.getElementById("content");
                                zz.appendChild(lineb);
                                zz.appendChild(header);
                               
                            

                            }


                            if (cnt % 5 == 0 || cnt==1)
                            {
                               
                var div = document.createElement('div');
                div.setAttribute('class', 'row');
                //div.textContent = "Sup, y'all?";
                                div.setAttribute('id', 'row' + i)

            }
                              
                                
                            
                            var div2 = document.createElement('div')
                            div2.setAttribute('class', 'col-sm-6 col-xl-3');

                            var div3 = document.createElement('div')
                            div3.setAttribute('class', 'card card-body text-center');


                            var heading = document.createElement('h6')
                            heading.setAttribute('class', 'font-weight-semibold mb-0 mt-1');
                            heading.textContent = dept[i];
                            div3.appendChild(heading)
                            var department = document.createElement('div')
                           
                            department.setAttribute('class', 'svg-center');
                            department.setAttribute('id', 'obj'+i);
                            div3.appendChild(department)
                           
                            div2.appendChild(div3)
                            div.appendChild(div2)
                            //var element = document.getElementById("pie_multiples");
                            //element.appendChild(div);
                          

                               
                            var zz = document.getElementById("content");
                            zz.appendChild(div);
                            cnt++;
                          
                            
                            _MultipleDonutWithLegend("#" + "obj" + i, 120, data);
                           

                         

                        }
                      
                      
                    
               
           
        };

       
      

        //-----------------donut widge creation


        // Donut with legend
        var _animatedMultipleDonutWithLegend2 = function (data) {
            if (typeof d3 == 'undefined') {
                console.warn('Warning - d3.min.js is not loaded.');
                return;
            }  
                    
                    var dept = [];
                    var Extreme = [];
                    var High = [];
                    var Medium = [];
                    var Low = [];

                    $.each(data, function (key, item) {


                        dept.push(item.deptname);
                        Extreme.push(item.Extreme);
                        High.push(item.High);
                        Medium.push(item.Medium);
                        Low.push(item.Low);

                    });
                    // Add data set

                    var c = dept.length;
                   
                    var data = [];
                    var cnt = 1;
                    for (var i = 0; i < c; i++) {
                        data = [];
                        data.push({
                            status: "Extreme",
                            value: Extreme[i],
                            color: getRandomColor()
                        });

                        data.push({
                            status: "High",
                            value: High[i],
                            color: getRandomColor()
                        });


                        data.push({
                            status: "Medium",
                            value: Medium[i],
                            color: getRandomColor()
                        });


                        data.push({
                            status: "Low",
                            value: Low[i],
                            color: getRandomColor()
                        });
                       


                        if (cnt == 1) {

                            var lineb = document.createElement('hr')
                            var header = document.createElement('div')
                            header.setAttribute('class', 'mb-3');
                            var headingmain = document.createElement('h6')
                            headingmain.setAttribute('class', 'font-weight-semibold mb-0 mt-1');
                            headingmain.textContent = "Risks By Department";
                            header.appendChild(headingmain)
                            var zz = document.getElementById("content");
                            zz.appendChild(lineb);
                            zz.appendChild(header);



                        }


                        if (cnt % 5 == 0 || cnt == 1) {
                           
                            var div = document.createElement('div');
                            div.setAttribute('class', 'row');
                            //div.textContent = "Sup, y'all?";
                            div.setAttribute('id', 'row' + i)

                        }



                        var div2 = document.createElement('div')
                        div2.setAttribute('class', 'col-sm-6 col-xl-3');

                        var div3 = document.createElement('div')
                        div3.setAttribute('class', 'card card-body text-center');


                        var heading = document.createElement('h6')
                        heading.setAttribute('class', 'font-weight-semibold mb-0 mt-1');
                        heading.textContent = dept[i];
                        div3.appendChild(heading)
                        var department = document.createElement('div')

                        department.setAttribute('class', 'svg-center');
                        department.setAttribute('id', 'risk' + i);
                        div3.appendChild(department)

                        div2.appendChild(div3)
                        div.appendChild(div2)
                        //var element = document.getElementById("pie_multiples");
                        //element.appendChild(div);



                        var zz = document.getElementById("content");
                        zz.appendChild(div);
                        cnt++;


                        _MultipleDonutWithLegend("#" + "risk" + i, 120, data);

                    }                
        };

      

        var _HseInspection = function (data) {
            var hseinsp_element = document.getElementById('hseinsp');
            if (hseinsp_element) {

                // Initialize chart
                var hseinspchart = echarts.init(hseinsp_element);

                var month = []; //month
                var total = [];

                $.each(data, function (key, item) {

                    month.push(item.MonthName);
                    total.push(item.No_inspections);

                });

                hseinspchart.setOption({

                    // Define colors
                    color: [
                        '#c05050', '#b6a2df', '#5ab1ef', '#ffb980', '#d87a80',
                        '#8d98b3', '#e5cf0d', '#97b552', '#95706d', '#dc69aa',
                        '#07a2a4', '#9a7fd1', '#588dd5', '#f5994e', '#c05050',
                        '#59678c', '#c9ab00', '#7eb00a', '#6f5553', '#c14089'
                    ],

                    // Global text styles
                    textStyle: {
                        fontFamily: 'Roboto, Arial, Verdana, sans-serif',
                        fontSize: 13
                    },

                    // Chart animation duration
                    animationDuration: 750,

                    // Setup grid
                    grid: {
                        left: 0,
                        right: 40,
                        top: 35,
                        bottom: 0,
                        containLabel: true
                    },

                    // Add legend
                    legend: {
                        show: false,
                        data: month,
                        itemHeight: 8,
                        itemGap: 20,
                        textStyle: {
                            padding: [0, 5]
                        }
                    },

                    // Add tooltip
                    tooltip: {
                        trigger: 'axis',
                        backgroundColor: 'rgba(0,0,0,0.75)',
                        padding: [10, 15],
                        textStyle: {
                            fontSize: 13,
                            fontFamily: 'Roboto, sans-serif'
                        }

                    },

                    // Horizontal axis
                    xAxis: [{
                        type: 'category',
                        data: month,
                        axisLabel: {
                            color: '#333'
                        },
                        axisLine: {
                            lineStyle: {
                                color: '#999'
                            }
                        },
                        splitLine: {
                            show: true,
                            lineStyle: {
                                color: '#eee',
                                type: 'dashed'
                            }
                        }
                    }],

                    // Vertical axis
                    yAxis: [{
                        type: 'value',
                        axisLabel: {
                            color: '#333'
                        },
                        axisLine: {
                            lineStyle: {
                                color: '#999'
                            }
                        },
                        splitLine: {
                            lineStyle: {
                                color: ['#eee']
                            }
                        },
                        splitArea: {
                            show: true,
                            areaStyle: {
                                color: ['rgba(250,250,250,0.1)', 'rgba(0,0,0,0.01)']
                            }
                        }
                    }],

                    // Add series
                    series: [
                        {
                            //name: doctype,
                            type: 'bar',
                            data: total,
                            itemStyle: {
                                normal: {
                                    label: {
                                        show: true,
                                        position: 'top',
                                        textStyle: {
                                            fontWeight: 500
                                        }
                                    }
                                }
                            }

                            //markLine: {
                            //    data: [{ type: 'average', name: 'Average' }]
                            //}
                        }

                    ]
                });
            }

            return hseinspchart;
       
        }
                                   
      
        //--- Maintenance
        var _Maintenance = function (data) {

            var Maintenance_element = document.getElementById('BPMaintenanceV');
            var objchart = echarts.init(Maintenance_element);
            var MonthName = [];
            var BM = [];
            var PM = [];  


            $.each(data, function (key, item) {


                MonthName.push(item.monthname);
                BM.push(item.bm);
                PM.push(item.pm);

            });

            objchart.setOption({

                // Define colors
                color: ['#2ec7c9', '#b6a2de', '#d87a80', '#ffb980', '#d87a80'],

                // Global text styles
                textStyle: {
                    fontFamily: 'Roboto, Arial, Verdana, sans-serif',
                    fontSize: 13
                },

                // Chart animation duration
                animationDuration: 750,

                // Setup grid
                grid: {
                    left: 0,
                    right: 40,
                    top: 35,
                    bottom: 0,
                    containLabel: true
                },

                // Add legend
                legend: {
                    data: ['Breakdown Maintenance', 'Prev Maintenance'],
                    itemHeight: 8,
                    itemGap: 20,
                    textStyle: {
                        padding: [0, 5]
                    }
                },

                // Add tooltip
                tooltip: {
                    trigger: 'axis',
                    backgroundColor: 'rgba(0,0,0,0.75)',
                    padding: [10, 15],
                    textStyle: {
                        fontSize: 13,
                        fontFamily: 'Roboto, sans-serif'
                    }
                },

                // Horizontal axis
                xAxis: [{
                    type: 'category',
                    data: MonthName,
                    axisLabel: {
                        color: '#333'
                    },
                    axisLine: {
                        lineStyle: {
                            color: '#999'
                        }
                    },
                    splitLine: {
                        show: true,
                        lineStyle: {
                            color: '#eee',
                            type: 'dashed'
                        }
                    }
                }],

                // Vertical axis
                yAxis: [{
                    type: 'value',
                    axisLabel: {
                        color: '#333'
                    },
                    axisLine: {
                        lineStyle: {
                            color: '#999'
                        }
                    },
                    splitLine: {
                        lineStyle: {
                            color: ['#eee']
                        }
                    },
                    splitArea: {
                        show: true,
                        areaStyle: {
                            color: ['rgba(250,250,250,0.1)', 'rgba(0,0,0,0.01)']
                        }
                    }
                }],

                // Add series
                series: [
                    {
                        name: 'Breakdown Maintenance',
                        type: 'bar',
                        data: BM,
                        itemStyle: {
                            normal: {
                                label: {
                                    show: true,
                                    position: 'top',
                                    textStyle: {
                                        fontWeight: 500
                                    }
                                }
                            }
                        },

                    },
                    {
                        name: 'Prev Maintenance',
                        type: 'bar',
                        data: PM,
                        itemStyle: {
                            normal: {
                                label: {
                                    show: true,
                                    position: 'top',
                                    textStyle: {
                                        fontWeight: 500
                                    }
                                }
                            }
                        },

                    }
                ]
            });

            return objchart;

        }

      
    }
   


           


 


};


document.addEventListener('DOMContentLoaded', function () {
    EchartsLoader.init();
  

});