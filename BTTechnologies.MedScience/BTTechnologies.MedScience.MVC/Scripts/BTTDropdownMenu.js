$(".BTT-dropdown-menu > ul").slideUp();
$(".BTT-dropdown-menu").hover(function () { // Hover
    var left = $(".BTT-dropdown-menu > span").offset().left;
    var top = $(".BTT-dropdown-menu > span").offset().top;
    var height = $(".BTT-dropdown-menu > span").height();
    var ul = $(this).children("ul");
    $(ul).css({ left: left, top: top + height });
    $(this).children("ul").slideDown();
},
function () { // Leave
    $(this).children("ul").hide().slideUp();
});