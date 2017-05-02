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
		var val;

        function addSeperator() {
            if (!qs.length) {
                if (includeQuestionMark) {
                    qs = qs + '?';
                }
            } else {
                qs = qs + '&';
            }
        }

        for (var i = 0; i < parameterInfos.length; ++i) {
            var parameterInfo = parameterInfos[i];
            if (parameterInfo.value === undefined) {
                continue;
            }

            if (parameterInfo.value === null) {
                parameterInfo.value = '';
            }

            addSeperator();

            if (parameterInfo.value.toJSON && typeof parameterInfo.value.toJSON === "function") {
                qs = qs + parameterInfo.name + '=' + encodeURIComponent(parameterInfo.value.toJSON());
            } else {
				if (Array.isArray(parameterInfo.value))//ids=24041&ids=24117
				{
					for(var j=0; j<parameterInfo.value.length; j++){
						if (j > 0) {
							addSeperator();
						}
					
						if (parameterInfo.value[j].toJSON && typeof parameterInfo.value[j].toJSON === "function")
							qs = qs + parameterInfo.name + '[' + j + ']=' + parameterInfo.value[j].toJSON();
						else
							qs = qs + parameterInfo.name + '[' + j + ']=' + encodeURIComponent(parameterInfo.value[j]);
					}
				} else {
					qs = qs + parameterInfo.name + '=' + encodeURIComponent(parameterInfo.value);
				}
            }
        }

        return qs;
    }


})();