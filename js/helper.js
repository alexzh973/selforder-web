$(function () {

    var current_index = null;

    $('#help-button').click(function () {
        $('#help-panel').toggle(150);
    });


    $('a.title').click(function () {

        if (current_index != $('a.title').index(this)) {
            $('.collapse:visible').slideUp(300);
            $('a.title').removeClass('selected');
        }

        if ($(this).parent().next().attr('class') == 'collapse') {

            if ($(this).parent().next().is(":hidden")) {
                $(this).addClass('selected');
            } else if ($(this).parent().next().is(":visible")) {
                $(this).removeClass('selected');
            }
            $(this).parent().next().slideToggle(300);
        }

        current_index = $('a.title').index(this);
    });
});