(function ($) {
    $.fn.userusagesummary = function () {
        return this.each(function () {
            var $this = $(this);

            $("form").on("submit", function (e) {
                $(".report-controls", $this).hide();
                $(".spinner", $this).show();
            });
        });
    };
}(jQuery));

