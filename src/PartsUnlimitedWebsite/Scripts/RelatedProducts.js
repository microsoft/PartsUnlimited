function getRelatedProducts(productId) {
    $.ajax({
        url: "/RelatedProducts/GetRelatedProducts",
        type: "GET",
        data: { productId: productId },
        success: function (data) {
            $("#relatedproducts-panel").html(data);
        },
        error: function (error) {
            alert("Failed to get related products...");
        },
    })
}