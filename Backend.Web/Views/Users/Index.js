(function () {

    $(function () {

        var tolenHeaderName = abp.security.antiForgery.tokenHeaderName;
        var token = abp.security.antiForgery.getToken();

        var _userService = abp.services.app.user;

        var _permissions = {
            create: abp.auth.hasPermission("Pages.Administration.Users.Create"),
            edit: abp.auth.hasPermission('Pages.Administration.Users.Edit'),
            'delete': abp.auth.hasPermission('Pages.Administration.Users.Delete')
        }

        var _createOrEditModal = app.ModalManager({
            viewUrl: abp.appPath + "Users/CreateOrEditModal",
            scriptUrl: abp.appPath + "Views/Users/_CreateOrEditModal.js",
            modalClass: 'CreateOrEditUserModal'
        });

        var _userPermissionsModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Users/PermissionsModal',
            scriptUrl: abp.appPath + 'Views/Users/_PermissionsModal.js',
            modalClass: 'UserPermissionsModal'
        });

        $.fn.dataTable.ext.errMode = 'none';

        // init date tables
        var userTable = $("#UserTable")
            .on("processing.dt", function (e, setttings, processing) {
                if (processing) {
                    abp.ui.setBusy('#UserTable');
                } else {
                    abp.ui.clearBusy('#UserTable');
                }
            })
            .on("error.dt", function (e, settings, techNote, message) {
                abp.message.error('加载数据出错!', '系统提示');
            })
            .on("draw.dt", function() {
                $("#UserTable :button.edit").on("click",
                    function () {
                        var data = userTable.api().data();
                        var index = $(this).attr('data-index');
                        _createOrEditModal.open({ id: data[index].id });
                    });
                $("#UserTable :button.permisstions").on("click",
                    function () {
                        var data = userTable.api().data();
                        var index = $(this).attr('data-index');
                        _userPermissionsModal.open({ id: data[index].id });
                    });
                $("#UserTable :button.delete").on("click",
                    function () {
                        var data = userTable.api().data();
                        var index = $(this).attr('data-index');
                        deleteUser(data[index]);
                    });
                $("#UserTable :button.unlock").on("click",
                    function () {
                        var data = userTable.api().data();
                        var index = $(this).attr('data-index');
                        unlockUser(data[index]);
                    });
            })
            .dataTable({
                "deferRender": true,
                "processing": false,
                "serverSide": true,
                "ajax": {
                    url: "/Users/List",
                    type: "post",
                    data: function (d) {
                        var obj = {};
                        obj.filter = $('#FilterText').val();
                        obj.draw = d.draw;
                        obj.start = d.start;
                        obj.length = d.length;
                        return obj;
                    }
                },
                "searching": false,
                "ordering": false,
                "scrollX": true, // X轴滚动条
                "columns": [
                    { "data": 'username', "visible": true },
                    {
                        "data": 'roles',
                        "visible": true,
                        render: function (data, type, row, meta) {
                            var roleNames = "";
                            for (var i = 0; i < data.length; i++) {
                                if (roleNames.length) {
                                    roleNames = roleNames + ",";
                                }
                                roleNames = roleNames + data[i].roleName;
                            }
                            return roleNames;
                        }
                    },
                    { "data": 'emailAddress', "visible": true },
                    {
                        "data": 'isEmailConfirmed',
                        "visible": true,
                        render: function (data, type, row, meta) {
                            if (data) {
                                return '<span class="label label-success">是</span>';
                            } else {
                                return '<span class="label label-default">否</span>';
                            }
                        }
                    },
                    {
                        "data": 'isActive', "visible": true, render: function (data, type, row, meta) {
                            if (data) {
                                return '<span class="label label-success">是</span>';
                            } else {
                                return '<span class="label label-default">否</span>';
                            }
                        }
                    },
                    {
                        "data": 'lastLoginTime',
                        "visible": true,
                        render: function (data, type, row, meta) {
                            return format(data);
                        }
                    },
                    {
                        "data": 'creationTime', "visible": true,
                        render: function (data, type, row, meta) {
                            return format(data);
                        }
                    },
                    {
                        "data": 'opt',
                        "visible": true,
                        render: function(data, type, row, meta) {
                            var opt = [];
                            if (_permissions.edit) {
                                opt.push('<button type="button" data-index=' +
                                    meta.row +
                                    ' class="btn btn-xs btn-flat edit">修改</button>');
                                opt.push('<button type="button" data-index=' +
                                    meta.row +
                                    ' class="btn btn-xs btn-flat permisstions">权限</button>');
                                opt.push('<button type="button" data-index=' +
                                    meta.row +
                                    ' class="btn btn-xs btn-flat unlock">解锁</button>');
                            }
                            if (_permissions.delete) {
                                opt.push('<button type="button" data-index=' +
                                    meta.row +
                                    ' class="btn btn-xs btn-flat delete">删除</button>');
                            }
                            return opt.join('');
                        }
                    }
                ],
                "language": {
                    "sProcessing": "处理中...",
                    "sLengthMenu": "每页 _MENU_ 条记录",
                    "sZeroRecords": "没有匹配结果",
                    "sInfo": "第 _PAGE_ 页 ( 总共 _PAGES_ 页 )",
                    "sInfoEmpty": "",
                    "sInfoFiltered": "(由 _MAX_ 项结果过滤)",
                    "sInfoPostFix": "",
                    "sSearch": "搜索:",
                    "sUrl": "",
                    "sEmptyTable": "没有查找到数据",
                    "sLoadingRecords": "载入中...",
                    "sInfoThousands": ",",
                    "oPaginate": {
                        "sFirst": "首页",
                        "sPrevious": "上页",
                        "sNext": "下页",
                        "sLast": "末页"
                    },
                    "oAria": {
                        "sSortAscending": ": 以升序排列此列",
                        "sSortDescending": ": 以降序排列此列"
                    }
                }
            });

        $("#SearchButton, #AndvanceSearchButton").on("click", function (e) {
            e.preventDefault();
            userTable.api().draw();
        });

        $('#CreateNewUserButton').on("click", function (e) {
            e.preventDefault();
            _createOrEditModal.open();
        });

        abp.event.on('app.createOrEditUserModalSaved', function () {
            userTable.api().draw();
        });

        function deleteUser(user) {
            if (user.username == app.consts.userManagement.defaultAdminUserName) {
                abp.message.warn(abp.utils.formatString("{0}用户不能被删除", app.consts.userManagement.defaultAdminUserName));
                return;
            }

            abp.message.confirm(abp.utils.formatString("确定要删除{0}吗?", user.username), function (isConfirmed) {
                if (isConfirmed) {
                    _userService.deleteUser({
                            id: user.id
                        })
                        .then(function() {
                            userTable.api().draw();
                            abp.notify.info("删除成功");
                        });
                }
            });
        }

        function unlockUser(user) {
            _userService.unlockUser({
                id: user.id
            }).done(function() {
                abp.notify.info("解锁成功");
            });
        }

        function format(date) {
            if (date) {
                return moment(date).format("L");
            }
            return '-';
        }

    });
})();