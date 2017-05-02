(function ($) {
    $.fn.userusagesummary = function () {
        return this.each(function () {
            var $this = $(this);

            $(".report-button", $this).click(function (e) {
                var btn = $(this);
                var container = btn.closest(".report-controls");
                container.hide();
                $(".spinner", $this).show();
            });

        });
    };
}(jQuery));

