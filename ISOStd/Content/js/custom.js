$(document).ready(function() {
	
    $('input,textarea').focus(function() {
        $(this).data('placeholder', $(this).attr('placeholder'))
        $(this).attr('placeholder', '');
    });
    $('input,textarea').blur(function() {
        $(this).attr('placeholder', $(this).data('placeholder'));
    });

    function backalert() {
        $("#tablealert").show();
        $("#divdetails").hide();
        $("#leftMenuMonitor").show();
    }
	$("#mytable td").on('click', function(e) {
		$("#ticket-dtl").show();
		$("#current-tbl").hide();
	});
	$("#back-cur-tkt").on('click', function(e) {
		$("#ticket-dtl").hide();
		$("#current-tbl").show();
	});
	$("#approv-btn").on('click', function(e) {
		$("#for-approv").hide();
		$("#tkt-confg").show();
	});
	$("#btn-update").on('click', function(e) {
		$("#tkt-confg").hide();
		$("#tkt-admin").show();
	});
	
 
    $(function() {
        $('#sidemenu a').on('click', function(e) {
            e.preventDefault();

            if ($(this).hasClass('open')) {
                // do nothing because the link is already open
            } else {
                var oldcontent = $('#sidemenu a.open').attr('href');
                var newcontent = $(this).attr('href');

                $(oldcontent).fadeOut('fast', function() {
                    $(newcontent).fadeIn().removeClass('hidden');
                    $(oldcontent).addClass('hidden');
                });


                $('#sidemenu a').removeClass('open');
                $(this).addClass('open');
            }
        });
    });
    $(function() {
        $('.tree li:has(ul)').addClass('parent_li').find(' > span').attr('title', 'Collapse this branch');
        $('.tree li.parent_li > span').on('click', function(e) {
            var children = $(this).parent('li.parent_li').find(' > ul > li');
            if (children.is(":visible")) {
                children.hide('fast');
                $(this).attr('title', 'Expand this branch').find(' > i').addClass('fa-caret-right').removeClass('fa-caret-down');
            } else {
                children.show('fast');
                $(this).attr('title', 'Collapse this branch').find(' > i').addClass('fa-caret-down').removeClass('fa-caret-right');
            }
            e.stopPropagation();
        });
    });

    $('.ip_address').mask('099.099.099.099', {
        placeholder: " "
    });
    $('.collapse').on('shown.bs.collapse', function() {
        $(this).parent().find(".fa-angle-down").removeClass("fa-angle-down").addClass("fa-angle-up");
    }).on('hidden.bs.collapse', function() {
        $(this).parent().find(".fa-angle-up").removeClass("fa-angle-up").addClass("fa-angle-down");
    });
  


 
    $("ul.subtab li a").click(function() {
        $("ul.subtab li a").removeClass("active");
        $(this).addClass("active");
        $(".tab_content").hide();
        var activeTab = $(this).attr("rel");
        $("#" + activeTab).fadeIn();
    });
	$('#myTab a[href="#divCurrentTicket"]').tab('show');
    $('#horizontalTab').responsiveTabs({
        rotate: false,
        startCollapsed: 'accordion',
        collapsible: 'accordion',
        setHash: true,
        activate: function(e, tab) {
            //$('.info').html('Tab <strong>' + tab.id + '</strong> activated!');
        },
        activateState: function(e, state) {
            //console.log(state);
            $('.info').html('Switched from <strong>' + state.oldState + '</strong> state to <strong>' + state.newState + '</strong> state!');
        }
    });
    $('#horizontalTab').responsiveTabs('activate', 1);
    $('#start-rotation').on('click', function() {
        $('#horizontalTab').responsiveTabs('startRotation', 1000);
    });
    $('#stop-rotation').on('click', function() {
        $('#horizontalTab').responsiveTabs('stopRotation');
    });
    $('#start-rotation').on('click', function() {
        $('#horizontalTab').responsiveTabs('active');
    });
    $('.select-tab').on('click', function() {
        $('#horizontalTab').responsiveTabs('activate', $(this).val());
    });

    $('#click_advance').click(function() {
        $('#display_advance').toggle('1000');
        icon = $(this).find("i");
        icon.toggleClass("fa-chevron-up fa-chevron-down");
        $('#click_advance').toggleClass('toggle-arrow1', 'linear');
    })
    $('#click_advance1').click(function() {
        $('#display_advance1').toggle('1000');
        icon = $(this).find("i");
        icon.toggleClass("fa-chevron-up fa-chevron-down");
        $('#click_advance1').toggleClass('toggle-arrow1', 'linear');
    })
    $("#datepicker-icon, #datepicker-icon1").datepicker({
        dateFormat: 'dd M yy'
    });
    $('[data-datepicker]').click(function(e) {
        var data = $(this).data('datepicker');
        $(data).focus();
    });
    $('[data-datepicker1]').click(function(e) {
        var data = $(this).data('datepicker');
        $(data).focus();
    });
    $('.selectpicker').selectpicker();


    setCarouselHeight('#carousel-example');

    function setCarouselHeight(id) {
        var slideHeight = [];
        $(id + ' .item').each(function() {
            // add all slide heights to an array
            slideHeight.push($(this).height());
        });

        // find the tallest item
        max = Math.max.apply(null, slideHeight);

        // set the slide's height
        $(id + ' .carousel-content').each(function() {
            $(this).css('height', max + 'px');
        });
    }
    $("#tablealert tr").click(function() {
        $("#divdetails").show();
        $("#leftMenuMonitor").hide();
    });

	$('#current-tbl').each(function(){  
            
            var highestBox = 0;
            $('#gridtable', this).each(function(){
            
                if($(this).height() > highestBox) 
                   highestBox = $(this).height(); 
            });  
            
            $('.col-md-1',this).height(highestBox);
            
        
    });
});


$(window).load(function() {
    $(".ticket_cat_rightcol").mCustomScrollbar({
        setHeight: 380,
        theme: "dark-3"
    });
    $(".ticket_cat_leftcol").mCustomScrollbar({
        setHeight: 380,
        theme: "dark-3"
    });
});

function adjustModalMaxHeightAndPosition() {
    $('.modal').each(function() {
        if ($(this).hasClass('in') == false) {
            $(this).show();
        };
        var contentHeight = $(window).height() - 60;
        var headerHeight = $(this).find('.modal-header').outerHeight() || 2;
        var footerHeight = $(this).find('.modal-footer').outerHeight() || 2;

        $(this).find('.modal-content').css({
            'max-height': function() {
                return contentHeight;
            }
        });

        $(this).find('.modal-body').css({
            'max-height': function() {
                return (contentHeight - (headerHeight + footerHeight));
            }
        });

        $(this).find('.modal-dialog').css({
            'margin-top': function() {
                return -($(this).outerHeight() / 2);
            },
            'margin-left': function() {
                return -($(this).outerWidth() / 2);
            }
        });
        if ($(this).hasClass('in') == false) {
            $(this).hide();
        };
    });
};

$(window).resize(adjustModalMaxHeightAndPosition).trigger("resize");