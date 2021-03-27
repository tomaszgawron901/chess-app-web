window.chessAppJsFunctions = {
    setSelect: function (component) {
        component.select();
    },

    setSelectAndCopy: function (component) {
        component.select();
        document.execCommand("copy");
    }
}