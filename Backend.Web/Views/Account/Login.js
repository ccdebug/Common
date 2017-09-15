(function() {

    $(function() {
        var $loginForm = $('#LoginForm');

        $loginForm.submit(function (e) {
            e.preventDefault();

            if (!$loginForm.valid()) {
                return;
            }

            abp.ui.setBusy(
                $('#LoginForm'),

                abp.ajax({
                    contentType: 'application/x-www-form-urlencoded',
                    url: $loginForm.attr('action'),
                    data: $loginForm.serialize()
                })
            );
        });

        $('#ReturnUrlHash').val(location.hash);

        $('#LoginForm input:first-child').focus();

    });

})();