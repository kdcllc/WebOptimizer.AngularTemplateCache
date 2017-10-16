app.controller("otherCtrl", function () {
    this.formData = {
        name: "",
        lastname: ""
    };
    this.submitForm = function (data) {
        alert("Form submitted with " + data.name + " " + data.lastname);
    };
});
//# sourceMappingURL=other.js.map