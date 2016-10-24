(function () {

    $(function () {
        $('#SubmitButton').click(function (e) {
            e.preventDefault();
            abp.ui.setBusy(
                $('.auth-block'),
                abp.ajax({
                    url: abp.appPath + 'Manage/SetPassword',
                    type: 'POST',
                    data: JSON.stringify({
                        email: $('#NewPassword').val(),
                        password: $('#ConfirmPassword').val(),
                    })
                })
            );
        });

        $('.auth-block input:first-child').focus();
    });

})();