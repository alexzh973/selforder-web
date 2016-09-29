tinyMCE.init({
    mode: "exact",
    elements: "txText",

    theme: "advanced",
    plugins: "pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,inlinepopups,autosave",

    // Theme options
    theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,fontselect,fontsizeselect,|,sub,sup,|,forecolor,backcolor,|,removeformat,code,undo,redo,|",
    theme_advanced_buttons2: "bullist,numlist,|,justifyleft,justifycenter,justifyright,justifyfull,|,outdent,indent,|,link,unlink,cleanup,|,hr,charmap,emotions,|",
    theme_advanced_buttons3: "",
    theme_advanced_toolbar_location: "top",
    theme_advanced_toolbar_align: "left",
    theme_advanced_statusbar_location: "none",
    theme_advanced_resizing: true,

    // Example word content CSS (should be your site CSS) this one removes paragraph margins
    //content_css: false,
    content_css: "../jscripts/tiny_mce/css/word.css",
    template_external_list_url: "../jscripts/tiny_mce/lists/template_list.js",
    external_link_list_url: "../jscripts/tiny_mce/lists/link_list.js",
    external_image_list_url: "../jscripts/tiny_mce/lists/image_list.js",
    media_external_list_url: "../jscripts/tiny_mce/lists/media_list.js",

    // Replace values for the template plugin
    template_replace_values: {
        username: "Some User",
        staffid: "991234"
    }
});