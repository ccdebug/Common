(function($) {

    app.modals.CreateOrEditUserModal = function() {
        var _modalManager;
        var _$userInformationForm = null;

        function _findAssignedRoleNames() {
            var assignedRoleNames = [];

            _modalManager.getModal()
                .find('#RolesTab input[type=checkbox]')
                .each(function() {
                    if ($(this).is(':checked')) {
                        assignedRoleNames.push($(this).attr('name'));
                    }
                });
        }

        this.init = function(modalManager) {
            _modalManager = modalManager;

            _$userInformationForm = _modalManager.getModal().find('form[name=UserInformationsForm]');
            _$userInformationForm.valid();

            $('input[type="checkbox"].minimal, input[type="radio"].minimal').iCheck({
                checkboxClass: 'icheckbox_minimal-blue',
                radioClass: 'iradio_minimal-blue'
            });
        }

        this.save = function() {
            if (!_$userInformationForm.valid()) {
                return;
            }

            var assignedRoleNames = _findAssignedRoleNames();
            var user = _$userInformationForm.serializeFormToObject();

            _modalManager.setBusy(true);
            abp.ajax({
                url: '/User/CreateOrUpdateUser',
                data: {
                    user: user,
                    assignedRoleNames: assignedRoleNames,
                    sendActivationEmail: user.SendActivationEmail,
                    SetRandomPassword: user.SetRandomPassword
                }
            }).done(function (data) {
                abp.notify.info('保存用户成功');
                _modalManager.close();
            }).always(function () {
                _modalManager.setBusy(false);
            });
        }
    }

})(jQuery);