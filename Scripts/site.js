(function($) {
    $(function() {
        var selectNumberDialog = $("#select-number-dialog");
        var to;
        selectNumberDialog.on("hidden.bs.modal", function() {
            var phoneNumber = selectNumberDialog.data("phone");
            if (!phoneNumber) return;
            $.post("/home/call", { from: phoneNumber, to: to }, function(r) {
                if (r && r.error) {
                    alert(r.error);
                }
            }, "json");
        });
        selectNumberDialog.find(".btn-number").on("click", function() {
            selectNumberDialog.data("phone", $(this).data("phone"));
            selectNumberDialog.modal("hide");
            return false;
        });
        selectNumberDialog.find(".btn-another-number").on("click", function() {
            selectNumberDialog.data("phone", selectNumberDialog.find("input[type=tel]").val());
            selectNumberDialog.modal("hide");
            return false;
        });
        $(".btn-call").on("click", function() {
            to = $(this).data("phone");
            selectNumberDialog.find("input[type=tel]").val("");
            selectNumberDialog.modal();
        });
        var btnAddContact = $(".btn-add-contact-form");
        var formAddContact = $(".form-add-contact");
        btnAddContact.on("click", function () {
            btnAddContact.hide();
            formAddContact.show();
        });
        formAddContact.on("submit", function () {
            var data = { name: formAddContact.find("input[name='name']").val(), phoneNumber: formAddContact.find("input[name='phoneNumber']").val() };
            $.post("/home/contact", data, function () {
                location.reload();
            }, "json");
            return false;
        });
    });
})(jQuery);