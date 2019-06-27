function includePayment(element) {
    $('.pay').toggleClass('show');
}

$(function () {
    $(document).on('click', '.delete', function (event) {
        if (!confirm("Izbriši podatak?")) {
            event.preventDefault();
        }
    });
});
