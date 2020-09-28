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