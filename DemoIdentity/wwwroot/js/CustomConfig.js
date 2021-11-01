CKEDITOR.editorConfig = function (config) {
    // bỏ 1 số tab
    config.removeDialogTabs = 'image:advanced;image:Link;link:advanced;link:upload';
    // thêm nút upload
    config.filebrowserImageUploadUrl = "/CKUploadImage/Upload";
};