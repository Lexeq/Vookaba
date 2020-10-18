jQuery.validator.unobtrusive.adapters.add(
    'requiredor', ['other'], function (options) {

        var getModelPrefix = function (fieldName) {
            return fieldName.substr(0, fieldName.lastIndexOf('.') + 1);
        }

        var prefix = getModelPrefix(options.element.name),
            other = options.params.other,
            fullOtherName = prefix + other,
            element = $(options.form).find(':input[name="' + fullOtherName + '"]')[0];

        options.rules['requiredor'] = element;
        if (options.message) {
            options.messages['requiredor'] = options.message;
        }
    }
);

jQuery.validator.addMethod('requiredor',
    function (value, element, params) {
        var otherValue = $(params).val();
        if (otherValue != null && otherValue != '') {
            return true;
        }
        return value != null && value != '';
    });


jQuery.validator.unobtrusive.adapters.addSingleVal('maxsize', 'size');

jQuery.validator.addMethod('maxsize',
    function (value, element, params) {
        if (element.files.length > 0) {
            return element.files.length == 1 && element.files[0].size <= params;
        }
        return true;
    });


jQuery.validator.unobtrusive.adapters.add('allowedFileTypes', ['types'], function (options) {
    options.rules['allowedFileTypes'] = options.params.types.split('|');
    if (options.message) {
        options.messages['allowedFileTypes'] = options.message;
    }
});

jQuery.validator.addMethod('allowedFileTypes',
    function (value, element, params) {
        let normalizedValue = value.toLowerCase();
        return element.files.length == 0 || params.some(e => normalizedValue.endsWith(e))
    });