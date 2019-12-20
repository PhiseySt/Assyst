var ButtonEdit = (function () {

    // Constructor
    function ButtonEdit(name, view, headerText, value, text, attributes) {
        this.name = name;
        this.view = view;
        this.headerText = headerText;
        this.value = value;
        this.text = text;
        this.attributes = attributes;
    }

    ButtonEdit.prototype.getValue = function () {
        return this.value;
    };

    ButtonEdit.prototype.getText = function () {
        return this.text;
    };

    ButtonEdit.prototype.setValue = function (value) {
        this.value = value;
        this.hdf.setValue(this.value);
    };

    ButtonEdit.prototype.setText = function (text) {
        this.text = text;
        this.be.setValue(this.text);
    };

    ButtonEdit.prototype.setEnabled = function (enabled) {
        this.be.setEnabled(enabled);
    };

    ButtonEdit.prototype.getAttribute = function (key) {
        return this.attributes[key];
    };

    ButtonEdit.prototype.setAttribute = function (key, value) {
        this.attributes[key] = value;
    };

    ButtonEdit.prototype.deleteAttribute = function (key) {
        delete this.attributes[key];
    };

    ButtonEdit.prototype.hdf_Init = function (hdf) {
        this.hdf = hdf;
        this.hdf.setValue(this.value);
    };

    ButtonEdit.prototype.hdf_ValueChanged = function () {
    };

    ButtonEdit.prototype.be_Init = function (be) {
        $(".buttonEdit input[name='be_" + this.name + "']").css('pointer-events', "none");
        this.be = be;
    };

    ButtonEdit.prototype.be_ButtonClick = function () {

        var url = addOrUpdateUrlParams("/Event/" + this.view + "?name=" + (this.text !== null ? this.text : ""), this.attributes);
        this.popup = createPopup({
            name: "popup_" + this.name,
            contentUrl: url,
            width: 900,
            height: 700,
            headerText: this.headerText,
            showFooter: false,
            submitButtonClickEvent: null,
            closeButtonClickEvent: null
        });
        this.popup.show();
        this.popup.callControl = this;
    };

    ButtonEdit.prototype.be_ValueChanged = function() {
        if (this.be.getValue() == null) {
            this.hdf.setValue(null);
            this.value = null;
            this.text = null;
            if (typeof window["buttonEdit_" + this.name + "_ValueChanged"] === 'function') {
                window["buttonEdit_" + this.name + "_ValueChanged"]();
            }
        }
        $(".buttonEdit input[name='be_" + this.name + "']").css('pointer-events', "none");
    };

    ButtonEdit.prototype.choiceElement = function (focusedRowKey) {
        this.popup.hide();
        var currentItem = focusedRowKey.toString();
        var delimiter = "|";
        var delimiterPosition = currentItem.indexOf(delimiter);
        var value = currentItem.split(delimiter)[0];
        var text = currentItem.slice(delimiterPosition + 1);
        this.setValue(value);
        this.setText(text);
        if (typeof window["buttonEdit_" + this.name + "_ValueChanged"] === 'function') {
            window["buttonEdit_" + this.name + "_ValueChanged"]();
        }
    };

    return ButtonEdit;
})();

function addOrUpdateUrlParams(url, attributes) {
    var newUrl = url;
    for (var key in attributes) {
        var regex = new RegExp("[&\\?]" + attributes[key] + "=");
        if (regex.test(newUrl)) {
            regex = new RegExp("([&\\?])" + attributes[key] + "=\\d+");
            newUrl = newUrl.replace(regex, "$1" + key + "=" + attributes[key]);
        } else {
            if (newUrl.indexOf("?") > -1)
                newUrl = newUrl + "&" + key + "=" + attributes[key];
            else
                newUrl = newUrl + "?" + key + "=" + attributes[key];
        }
    }
    return newUrl;
}









