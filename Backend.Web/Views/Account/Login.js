(function() {

    $(function() {
        var $loginForm = $('#LoginForm');

        $loginForm.submit(function (e) {
            e.preventDefault();

            if (!$loginForm.valid()) {
                return;
            }

            abp.ui.setBusy(
                $('#LoginArea'),

                abp.ajax({
                    contentType: 'application/x-www-form-urlencoded',
                    url: $loginForm.attr('action'),
                    data: $loginForm.serialize()
                })
            );
        });

        $('#ReturnUrlHash').val(location.hash);

        $('#LoginForm input:first-child').focus();

        $('input').iCheck({
            checkboxClass: 'icheckbox_square-blue',
            radioClass: 'iradio_square-blue',
            increaseArea: '20%' // optional
        });

    });

})();