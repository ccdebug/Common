(function($) {

    app.modals.CreateOrEditUserModal = function () {

        var _userService = abp.services.app.user;

        var _modalManager;
        var _$userInformationForm = null;

        function _findAssignedRoleNames() {
            var assignedRoleNames = [];

            console.log(_modalManager.getModal()
                .find('#RolesTab input[type=checkbox]'));

            _modalManager.getModal()
                .find('#RolesTab input[type=checkbox]')
                .each(function() {
                    if ($(this).is(':checked')) {
                        assignedRoleNames.push($(this).attr('name'));
                    }
                });

            return assignedRoleNames;

        }

        this.init = function(modalManager) {
            _modalManager = modalManager;

            _$userInformationForm = _modalManager.getModal().find('form[name=UserInformationsForm]');
            _$userInformationForm.valid();

            _modalManager.getModal()
                .find('#RolesTab input[type=checkbox]')
                .on('ifChanged', function () {
                    $('#assigned-role-count').text(_findAssignedRoleNames().length);
                });
               

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
            _userService.createOrUpdateUser({
                user: user,
                assignedRoleNames: assignedRoleNames,
                sendActivationEmail: false,
                SetRandomPassword: false
            }).done(function () {
                abp.notify.info('保存成功');
                _modalManager.close();
                abp.event.trigger('app.createOrEditUserModalSaved');
            }).always(function () {
                _modalManager.setBusy(false);
            });
        }
    }

})(jQuery);