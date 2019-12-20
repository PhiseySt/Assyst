function createPopup(settings) {
    window.top[settings.name] = new window.top.Popup(settings);
    window.top[settings.name].creatingWindow = window;
    return window.top[settings.name];
}

var Popup = (function () {

    // Constructor
    function Popup(settings) {
        this.settings = settings;
    }

    Popup.prototype.show = function () {
       
        var popup = this;
        $.ajax({
            type: 'POST',
            url: "/HtmlHelper/PopupPartialView/",
            data: popup.settings,
            dataType: "html",
            success: function (data) {
                $('body').append(data);
                popup.control = window["popup_" + popup.settings.name];
                popup.control.show();
            },
            error: function (data) {
               
            }
        });
    };

    Popup.prototype.setContentUrl = function (contentUrl) {
        this.settings.contentUrl = contentUrl;
        this.control.setContentUrl(contentUrl);
    };

    Popup.prototype.setHeaderText = function (headerText) {
        this.settings.headerText = headerText;
        this.control.setHeaderText(headerText);
    };

    Popup.prototype.setWidth = function (width) {
        this.settings.width = width;
        this.control.setWidth(width);
    };

    Popup.prototype.setHeight = function (height) {
        this.settings.height = height;
        this.control.setHeight(height);
    };

    Popup.prototype.hide = function () {
        this.control.hide();
    };

    Popup.prototype.popup_Closing = function (popup) {
        if (popup.settings.closingEvent != null && typeof popup.creatingWindow[popup.settings.closingEvent] === 'function') {
            popup.creatingWindow[popup.settings.closingEvent](popup);
        }
        $('.popup[name = "' + popup.settings.name + '"]').remove();
    };

    Popup.prototype.popup_Init = function (popup) {
        popup.getFrames()[0].onload = function () {
            popup.getFrames()[0].contentWindow["refPopup"] = popup;
        };
    };

    Popup.prototype.popup_CloseButtonClick = function (popup) {
        if (popup.settings.closeButtonClickEvent != null && typeof popup.creatingWindow[popup.settings.closeButtonClickEvent] === 'function') {
            popup.creatingWindow[popup.settings.closeButtonClickEvent](popup);
        }
    };

    Popup.prototype.popup_SubmitButtonClick = function (popup) {
        if (popup.settings.submitButtonClickEvent != null && typeof popup.creatingWindow[popup.settings.submitButtonClickEvent] === 'function') {
            popup.creatingWindow[popup.settings.submitButtonClickEvent](popup);
        }
    };

    Popup.prototype.getFrames = function () {
        return $("#popup_" + this.settings.name + " iframe");
    };

    return Popup;
})();








