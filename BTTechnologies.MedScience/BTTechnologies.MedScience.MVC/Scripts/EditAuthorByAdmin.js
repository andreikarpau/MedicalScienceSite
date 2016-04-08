function SelectAccountById(id, name) {
    $('#AccountName').val(name);
    $('#AccountId').val(id);
    $('ul.sub_menu').css('visibility', 'hidden');
}
