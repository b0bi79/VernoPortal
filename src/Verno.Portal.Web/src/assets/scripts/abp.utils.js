var abp = abp || {};
(function () {

    abp.utils = abp.utils || {};


    /**
     * parameterInfos should be an array of { name, value } objects
     * where name is query string parameter name and value is it's value.
     * includeQuestionMark is true by default.
     */
    abp.utils.buildQueryString = function (parameterInfos, includeQuestionMark) {
        if (includeQuestionMark === undefined) {
            includeQuestionMark = true;
        }

        var qs = '';

        for (var i = 0; i < parameterInfos.length; ++i) {
            var parameterInfo = parameterInfos[i];
            if (parameterInfo.value === undefined) {
                continue;
            }

            if (parameterInfo.value === null) {
                parameterInfo.value = '';
            }

            if (!qs.length) {
                if (includeQuestionMark) {
                    qs = qs + '?';
                }
            } else {
                qs = qs + '&';
            }

            if (parameterInfo.value.toJSON && typeof parameterInfo.value.toJSON === "function") {
                qs = qs + parameterInfo.name + '=' + encodeURIComponent(parameterInfo.value.toJSON());
            } else {
				if (Array.isArray(parameterInfo.value))//ids=24041&ids=24117
				{
					for(var i=0; i<parameterInfo.value.length; i++){
						qs = qs + parameterInfo.name + '=' + encodeURIComponent(parameterInfo.value[i]) + '&';
					}
					qs = qs.slice(0, -1);
				} else {
					qs = qs + parameterInfo.name + '=' + encodeURIComponent(parameterInfo.value);
				}
            }
        }

        return qs;
    }


})();