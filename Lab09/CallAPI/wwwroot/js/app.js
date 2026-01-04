const API_URL = 'https://localhost:44354/api/products';
const API_URLIMG = 'https://localhost:44354';

$(document).ready(function () {
    loadProducts();
    $('#addProductForm').on('submit', function (e) {
        e.preventDefault();
        saveProduct();
    });
    $("#productForm").submit(function (e) {
        e.preventDefault();
        debugger;
        var formData = new FormData();
        formData.append("name", $("#name").val());
        formData.append("price", $("#price").val());
        formData.append("image", $("#image")[0].files[0]);

        $.ajax({
            url: "https://localhost:44354/api/products",
            type: "POST",
            data: formData,
            processData: false, // ❗ bắt buộc
            contentType: false, // ❗ bắt buộc
            success: function (res, textStatus, xhr) {
                if (xhr.status === 201) {
                    $("#result").html("<p style='color:green'>Tạo sản phẩm thành công</p>");
                    loadProducts();
                }
            },
            error: function (xhr) {
                var response = xhr.responseJSON;
                handleError(xhr.status, response);
            }
        });
    });
});

function loadProducts() {
    $('#loading').show();
    $('#productList').html(''); 
    $.ajax({
        url: API_URL,
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            $('#loading').hide();

            if (response.statusCode != 200) {
                //alert("ahih");
                return;
            }
            if (response.products && response.products.length > 0) {
                displayProducts(response.products);
            } else {
                $('#productList').html('<p>Chưa có sản phẩm nào</p>');
            }
        },
        error: function (xhr, status, error) {
            $('#loading').hide();
            handleAjaxError(xhr, 'Lỗi khi tải danh sách sản phẩm');
        }
    });
}

function displayProducts(products) {
    let html = '<h2>Danh sách sản phẩm</h2>';

    products.forEach(function (product) {
        html += `
            <div class="product-card" id="product-${product.id}">
                <h3>${product.name}</h3>
                <p><strong>Giá:</strong> ${formatPrice(product.price)}</p>
                <p><strong>Mô tả:</strong> ${product.description || 'Không có mô tả'}</p>
                <img  src="${API_URLIMG}/${product.imageUrl}" alt="Loaded image will appear here" style="max-width: 100%;" />
                <button class="btn btn-primary" onclick="editProduct(${product.id})">Sửa</button>
                <button class="btn btn-danger" onclick="deleteProduct(${product.id})">Xóa</button>
            </div>
        `;
    });

    $('#productList').html(html);
}

function saveProduct() {
    const productId = $('#productId').val();
    const productData = {
        name: $('#productName').val(),
        price: parseFloat($('#productPrice').val()),
        description: $('#productDescription').val()
    };

    if (!productData.name || !productData.price) {
        showNotification('Vui lòng nhập đầy đủ thông tin', 'error');
        return;
    }

    if (productId) {
        updateProduct(productId, productData);
    } else {
        createProduct(productData);
    }
}

function createProduct(productData) {
    $.ajax({
        url: API_URL,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(productData),
        success: function (response, status, xhr) {
            if (xhr.status === 201) {
                showNotification('Tạo sản phẩm thành công!', 'success');
                resetForm();
                loadProducts();  
            }
        },
        error: function (xhr, status, error) {
            handleAjaxError(xhr, 'Lỗi khi tạo sản phẩm');
        }
    });
}

function editProduct(id) {
    $.ajax({
        url: `${API_URL}/${id}`,
        type: 'GET',
        dataType: 'json',
        success: function (product) {
            $('#productId').val(product.id);
            $('#productName').val(product.name);
            $('#productPrice').val(product.price);
            $('#productDescription').val(product.description);

            $('html, body').animate({
                scrollTop: $('#productForm').offset().top
            }, 500);

            $('#productForm h2').text('Cập nhật sản phẩm');
        },
        error: function (xhr, status, error) {
            handleAjaxError(xhr, 'Lỗi khi tải thông tin sản phẩm');
        }
    });
}

function updateProduct(id, productData) {
    $.ajax({
        url: `${API_URL}/${id}`,
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(productData),
        success: function (response, status, xhr) {
            if (xhr.status === 200 || xhr.status === 204) {
                showNotification('Cập nhật sản phẩm thành công!', 'success');
                resetForm();
                loadProducts();  
            }
        },
        error: function (xhr, status, error) {
            handleAjaxError(xhr, 'Lỗi khi cập nhật sản phẩm');
        }
    });
}
function deleteProduct(id) {
    if (!confirm('Bạn có chắc chắn muốn xóa sản phẩm này?')) {
        return;
    }

    $.ajax({
        url: `${API_URL}/${id}`,
        type: 'DELETE',
        success: function (response, status, xhr) {
            if (xhr.status === 204) {
                showNotification('Xóa sản phẩm thành công!', 'success');

                $(`#product-${id}`).fadeOut(400, function () {
                    $(this).remove();
                    if ($('.product-card').length === 0) {
                        $('#productList').html('<p>Chưa có sản phẩm nào</p>');
                    }
                });
            }
        },
        error: function (xhr, status, error) {
            handleAjaxError(xhr, 'Lỗi khi xóa sản phẩm');
        }
    });
}
function handleAjaxError(xhr, defaultMessage) {
    let message = defaultMessage;

    switch (xhr.status) {
        case 400: // Bad Request
            message = 'Dữ liệu không hợp lệ';
            if (xhr.responseJSON && xhr.responseJSON.message) {
                message += ': ' + xhr.responseJSON.message;
            }
            break;

        case 401: // Unauthorized
            message = 'Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại';
            // Redirect to login
            setTimeout(function () {
                window.location.href = '/login';
            }, 2000);
            break;

        case 403: // Forbidden
            message = 'Bạn không có quyền thực hiện hành động này';
            break;

        case 404: // Not Found
            message = 'Không tìm thấy sản phẩm';
            break;

        case 409: // Conflict
            message = 'Dữ liệu bị trùng lặp hoặc xung đột';
            break;

        case 500: // Internal Server Error
            message = 'Lỗi server. Vui lòng thử lại sau';
            break;

        case 503: // Service Unavailable
            message = 'Hệ thống đang bảo trì. Vui lòng quay lại sau';
            break;

        case 0: // Network error
            message = 'Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng';
            break;

        default:
            if (xhr.responseJSON && xhr.responseJSON.message) {
                message = xhr.responseJSON.message;
            }
    }

    showNotification(message, 'error');
    console.error('Ajax Error:', {
        status: xhr.status,
        statusText: xhr.statusText,
        responseText: xhr.responseText
    });
}

// ==================== HELPER FUNCTIONS ====================

function showNotification(message, type) {
    const className = type === 'error' ? 'error' : 'success';
    const html = `<div class="${className}">${message}</div>`;

    $('#notification').html(html);

    // Tự động ẩn sau 3 giây
    setTimeout(function () {
        $('#notification').fadeOut(400, function () {
            $(this).html('').show();
        });
    }, 3000);
}

function resetForm() {
    $('#addProductForm')[0].reset();
    $('#productId').val('');
    $('#productForm h2').text('Thêm sản phẩm mới');
}

function formatPrice(price) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(price);
}