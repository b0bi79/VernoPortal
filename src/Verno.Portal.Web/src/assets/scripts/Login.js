﻿(function () {

    function detectIE() {
        var ua = window.navigator.userAgent;

        var msie = ua.indexOf('MSIE ');
        if (msie > 0) {
            return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
        }

        var trident = ua.indexOf('Trident/');
        if (trident > 0) {
            var rv = ua.indexOf('rv:');
            return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
        }

        var edge = ua.indexOf('Edge/');
        if (edge > 0) {
            return parseInt(ua.substring(edge + 5, ua.indexOf('.', edge)), 10);
        }

        return false;
    }

    if (detectIE())
        $("#ie-alert").show();

    $(function () {
        /*$('#LoginButton').click(function (e) {
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
        });*/

        $('.auth-block input:first-child').focus();
    });

})();