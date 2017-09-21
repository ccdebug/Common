(function () {



    $(function () {

        var tolenHeaderName = abp.security.antiForgery.tokenHeaderName;
        var token = abp.security.antiForgery.getToken();

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

        $.fn.dataTable.ext.errMode = 'none';

        // init date tables
        var confTable = $("#UserTable")
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
                    {
                        "data": 'opt',
                        "visible": true,
                        render: function(data, type, row, meta) {
                            return '<div class="btn-group dropdown"><button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown"><i class="fa fa-cog"></i><span class="caret"></span>  <span class="sr-only">Toggle Dropdown</span></button><ul class="dropdown-menu" role="menu"><li><a href="#">修改</a></li><li><a href="#">权限</a><li><a href="#">解锁</a></li><li><a href="#">删除</a></li></ul></div>';
                        }
                    },
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
                                roleNames = roleNames + data[i].roleName
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
            confTable.api().draw();
        });

        $('#CreateNewUserButton').click(function (e) {
            e.preventDefault();
            _createOrEditModal.open();
        });

        function format(date) {
            if (date) {
                return moment().format("L");
            }
            return '-';
        }

    });
})();