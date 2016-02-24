$(function () {

    var ajaxFormSubmit = function () {
        var $form = $(this);

        var options = {
            url: $form.attr("action"),
            type: $form.attr("method"),
            data: $form.serialize()
        };

        $.ajax(options).done(function (data) {
            var $target = $($form.attr("data-uptime-target"));
            var $newHtml = $(data);
            $target.replaceWith($newHtml);
            $newHtml.effect("highlight");
        });

        return false;
    };


    var getPage = function () {
        var $a = $(this);

        var options = {
            url: $a.attr("href"),
            data: {newRequest: false},
            type: "get"
        };

        $.ajax(options).done(function (data) {
            var target = $a.parents("div.pagedList").attr("data-uptime-target");
            $(target).replaceWith(data);
        });
        return false;

    };


    var changeCurrency = function () {
        var $selected = $(this).val();


        var options = {
            url: "amazon/index",
            data: { newRequest: false, currency: $selected },
            type: "get"
        };


        $.ajax(options).done(function (data) {
            var $target = $("#itemList");
            var $newHtml = $(data);
            $target.replaceWith($newHtml);
            $newHtml.effect("highlight");
        });
        return false;

    };

    $("form[data-uptime-ajax='true']").submit(ajaxFormSubmit);

    $(".body-content").on("click", ".pagedList a", getPage);

    $(".body-content").on("change", "select", changeCurrency);

    
});