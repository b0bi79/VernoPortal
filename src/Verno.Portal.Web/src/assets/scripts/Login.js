(function () {

    $(function () {
        $('#LoginButton').click(function (e) {
            e.preventDefault();
            abp.ui.setBusy(
                $('.auth-block'),
                abp.ajax({
                    url: abp.appPath + 'Account/Login',
                    type: 'POST',
                    data: JSON.stringify({
                        email: $('#Email').val(),
                        password: $('#Password').val(),
                        rememberMe: $('#RememberMe').is(':checked'),
                        returnUrlHash: $('#ReturnUrlHash').val()
                    })
                })
            );
        });

        $('.auth-block input:first-child').focus();
    });

})();