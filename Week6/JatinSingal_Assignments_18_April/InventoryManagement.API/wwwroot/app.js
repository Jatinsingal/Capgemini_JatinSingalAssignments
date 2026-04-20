const apiBase = "/api";

const state = {
    token: localStorage.getItem("inventoryToken") || "",
    user: null,
    products: [],
    customers: [],
    orders: [],
    editingProductId: null,
    editingCustomerId: null,
    authMode: "login"
};

const refs = {
    authView: document.getElementById("authView"),
    appView: document.getElementById("appView"),
    authForm: document.getElementById("authForm"),
    authCopy: document.getElementById("authCopy"),
    authSubmit: document.getElementById("authSubmit"),
    loginTab: document.getElementById("loginTab"),
    registerTab: document.getElementById("registerTab"),
    usernameField: document.getElementById("usernameField"),
    mobileField: document.getElementById("mobileField"),
    roleField: document.getElementById("roleField"),
    email: document.getElementById("email"),
    password: document.getElementById("password"),
    username: document.getElementById("username"),
    mobileNumber: document.getElementById("mobileNumber"),
    userRole: document.getElementById("userRole"),
    profileName: document.getElementById("profileName"),
    profileRole: document.getElementById("profileRole"),
    welcomeText: document.getElementById("welcomeText"),
    logoutButton: document.getElementById("logoutButton"),
    refreshButton: document.getElementById("refreshButton"),
    statsGrid: document.getElementById("statsGrid"),
    lowStockList: document.getElementById("lowStockList"),
    recentOrdersList: document.getElementById("recentOrdersList"),
    productsTableBody: document.getElementById("productsTableBody"),
    customersTableBody: document.getElementById("customersTableBody"),
    ordersList: document.getElementById("ordersList"),
    toast: document.getElementById("toast"),
    productForm: document.getElementById("productForm"),
    productId: document.getElementById("productId"),
    productName: document.getElementById("productName"),
    productDescription: document.getElementById("productDescription"),
    productPrice: document.getElementById("productPrice"),
    productQuantity: document.getElementById("productQuantity"),
    productFormHeading: document.getElementById("productFormHeading"),
    productSubmitButton: document.getElementById("productSubmitButton"),
    productCancelButton: document.getElementById("productCancelButton"),
    productPermissionNote: document.getElementById("productPermissionNote"),
    customerForm: document.getElementById("customerForm"),
    customerId: document.getElementById("customerId"),
    customerName: document.getElementById("customerName"),
    customerEmail: document.getElementById("customerEmail"),
    customerPhone: document.getElementById("customerPhone"),
    customerAddress: document.getElementById("customerAddress"),
    customerFormHeading: document.getElementById("customerFormHeading"),
    customerCancelButton: document.getElementById("customerCancelButton"),
    orderForm: document.getElementById("orderForm"),
    orderCustomer: document.getElementById("orderCustomer"),
    orderLines: document.getElementById("orderLines"),
    addOrderLineButton: document.getElementById("addOrderLineButton"),
    orderTotalValue: document.getElementById("orderTotalValue")
};

let toastTimer = null;

document.addEventListener("DOMContentLoaded", initializeApp);

function initializeApp() {
    bindEvents();
    switchAuthMode("login");

    if (state.token) {
        restoreSession();
        return;
    }

    showAuthView();
}

function bindEvents() {
    refs.loginTab.addEventListener("click", () => switchAuthMode("login"));
    refs.registerTab.addEventListener("click", () => switchAuthMode("register"));
    refs.authForm.addEventListener("submit", handleAuthSubmit);
    refs.logoutButton.addEventListener("click", logout);
    refs.refreshButton.addEventListener("click", refreshDashboard);
    refs.productForm.addEventListener("submit", handleProductSubmit);
    refs.productCancelButton.addEventListener("click", resetProductForm);
    refs.customerForm.addEventListener("submit", handleCustomerSubmit);
    refs.customerCancelButton.addEventListener("click", resetCustomerForm);
    refs.orderForm.addEventListener("submit", handleOrderSubmit);
    refs.addOrderLineButton.addEventListener("click", () => addOrderLine());

    document.querySelectorAll(".nav-link").forEach((button) => {
        button.addEventListener("click", () => {
            document.querySelectorAll(".nav-link").forEach((link) => link.classList.remove("active"));
            button.classList.add("active");
            document.getElementById(button.dataset.target)?.scrollIntoView({ behavior: "smooth", block: "start" });
        });
    });
}

async function restoreSession() {
    try {
        state.user = await apiRequest("/me");
        showAppView();
        await refreshDashboard();
    } catch (error) {
        logout(false);
        showToast(error.message || "Session expired. Please sign in again.", "error");
    }
}

function switchAuthMode(mode) {
    state.authMode = mode;
    const isRegister = mode === "register";

    refs.loginTab.classList.toggle("active", !isRegister);
    refs.registerTab.classList.toggle("active", isRegister);
    refs.usernameField.hidden = !isRegister;
    refs.mobileField.hidden = !isRegister;
    refs.roleField.hidden = !isRegister;
    refs.authCopy.textContent = isRegister
        ? "Create an account to start working inside the inventory dashboard."
        : "Use your account to open the inventory workspace.";
    refs.authSubmit.textContent = isRegister ? "Create Account" : "Sign In";
}

async function handleAuthSubmit(event) {
    event.preventDefault();

    const payload = {
        email: refs.email.value.trim(),
        password: refs.password.value
    };

    const endpoint = state.authMode === "register" ? "/register" : "/login";

    if (state.authMode === "register") {
        payload.username = refs.username.value.trim();
        payload.mobileNumber = refs.mobileNumber.value.trim();
        payload.userRole = refs.userRole.value;
    }

    setButtonBusy(refs.authSubmit, true, state.authMode === "register" ? "Creating..." : "Signing in...");

    try {
        const result = await apiRequest(endpoint, {
            method: "POST",
            body: payload,
            authorize: false
        });

        if (state.authMode === "register") {
            showToast("Account created. Please sign in with your new credentials.", "success");
            refs.authForm.reset();
            refs.userRole.value = "Admin";
            switchAuthMode("login");
            return;
        }

        state.token = result.token;
        state.user = result.user;
        localStorage.setItem("inventoryToken", result.token);

        showAppView();
        await refreshDashboard();
        showToast("Login successful.", "success");
    } catch (error) {
        showToast(error.message || "Authentication failed.", "error");
    } finally {
        setButtonBusy(refs.authSubmit, false, state.authMode === "register" ? "Create Account" : "Sign In");
    }
}

function showAuthView() {
    refs.authView.classList.remove("hidden");
    refs.appView.classList.add("hidden");
}

function showAppView() {
    refs.authView.classList.add("hidden");
    refs.appView.classList.remove("hidden");
    renderProfile();
}

function renderProfile() {
    refs.profileName.textContent = state.user?.username || state.user?.email || "User";
    refs.profileRole.textContent = state.user?.userRole || "Role";
    refs.welcomeText.textContent = `Welcome ${state.user?.username || "there"}. Review inventory health, manage customer data, and process orders confidently.`;
    updateProductFormAccess();
}

function updateProductFormAccess() {
    const isAdmin = state.user?.userRole === "Admin";
    const disabled = !isAdmin;

    refs.productName.disabled = disabled;
    refs.productDescription.disabled = disabled;
    refs.productPrice.disabled = disabled;
    refs.productQuantity.disabled = disabled;
    refs.productSubmitButton.disabled = disabled;
    refs.productCancelButton.disabled = disabled;
    refs.productPermissionNote.textContent = isAdmin
        ? "Admins can create and edit products directly from this form."
        : "Inventory Managers can view products here. Product creation and updates are limited to Admin accounts.";
}

async function refreshDashboard() {
    setButtonBusy(refs.refreshButton, true, "Refreshing...");

    try {
        const [products, customers, orders] = await Promise.all([
            apiRequest("/product"),
            apiRequest("/customer"),
            apiRequest("/order")
        ]);

        state.products = Array.isArray(products) ? products : [];
        state.customers = Array.isArray(customers) ? customers : [];
        state.orders = Array.isArray(orders) ? orders : [];

        renderDashboard();
    } catch (error) {
        showToast(error.message || "Unable to refresh dashboard data.", "error");
    } finally {
        setButtonBusy(refs.refreshButton, false, "Refresh Data");
    }
}

function renderDashboard() {
    renderOverview();
    renderProducts();
    renderCustomers();
    renderOrders();
    populateOrderCustomerOptions();
    rebuildOrderLines();
    updateOrderTotal();
}

function renderOverview() {
    const lowStockProducts = state.products.filter((product) => product.quantity <= 10);
    const inventoryValue = state.products.reduce((sum, product) => sum + (Number(product.price) * Number(product.quantity)), 0);
    const revenue = state.orders.reduce((sum, order) => sum + Number(order.totalAmount), 0);

    const stats = [
        { label: "Products", value: state.products.length, note: `${lowStockProducts.length} need attention` },
        { label: "Customers", value: state.customers.length, note: "Active records ready for ordering" },
        { label: "Orders", value: state.orders.length, note: "Recent transaction history loaded" },
        { label: "Inventory Value", value: formatCurrency(inventoryValue), note: `Revenue ${formatCurrency(revenue)}` }
    ];

    refs.statsGrid.innerHTML = stats.map((stat) => `
        <article class="stat-card">
            <span class="stack-row-kicker">${escapeHtml(stat.label)}</span>
            <strong>${escapeHtml(String(stat.value))}</strong>
            <span class="stack-row-subtitle">${escapeHtml(stat.note)}</span>
        </article>
    `).join("");

    refs.lowStockList.innerHTML = lowStockProducts.length
        ? lowStockProducts
            .sort((a, b) => a.quantity - b.quantity)
            .slice(0, 5)
            .map((product) => `
                <article class="stack-row">
                    <div>
                        <span class="stack-row-kicker">Low stock</span>
                        <h5 class="stack-row-title">${escapeHtml(product.name)}</h5>
                        <p class="stack-row-subtitle">${escapeHtml(product.description || "No description added yet.")}</p>
                    </div>
                    <span class="status-pill ${getStockClass(product.quantity)}">${escapeHtml(getStockLabel(product.quantity))}</span>
                </article>
            `).join("")
        : `<div class="empty-state">All products are comfortably stocked right now.</div>`;

    refs.recentOrdersList.innerHTML = state.orders.length
        ? state.orders.slice(0, 5).map((order) => `
            <article class="stack-row">
                <div>
                    <span class="stack-row-kicker">Order #${escapeHtml(String(order.orderId))}</span>
                    <h5 class="stack-row-title">${escapeHtml(order.customerName || "Customer")}</h5>
                    <p class="stack-row-subtitle">${escapeHtml(formatDate(order.orderDate))}</p>
                </div>
                <strong>${escapeHtml(formatCurrency(order.totalAmount))}</strong>
            </article>
        `).join("")
        : `<div class="empty-state">No orders have been placed yet.</div>`;
}

function renderProducts() {
    const canManageProducts = state.user?.userRole === "Admin";

    refs.productsTableBody.innerHTML = state.products.length
        ? state.products.map((product) => `
            <tr>
                <td>${escapeHtml(product.name)}</td>
                <td>${escapeHtml(product.description || "No description")}</td>
                <td>${escapeHtml(formatCurrency(product.price))}</td>
                <td>${escapeHtml(String(product.quantity))}</td>
                <td><span class="status-pill ${getStockClass(product.quantity)}">${escapeHtml(getStockLabel(product.quantity))}</span></td>
                <td>
                    <div class="table-action-row">
                        <button class="table-button" type="button" ${canManageProducts ? "" : "disabled"} onclick="startProductEdit(${product.productId})">Edit</button>
                        <button class="table-button danger" type="button" ${canManageProducts ? "" : "disabled"} onclick="deleteProduct(${product.productId})">Delete</button>
                    </div>
                </td>
            </tr>
        `).join("")
        : `<tr><td colspan="6"><div class="empty-state">No products added yet.</div></td></tr>`;
}

function renderCustomers() {
    refs.customersTableBody.innerHTML = state.customers.length
        ? state.customers.map((customer) => `
            <tr>
                <td>${escapeHtml(customer.name)}</td>
                <td>${escapeHtml(customer.email || "-")}</td>
                <td>${escapeHtml(customer.phone || "-")}</td>
                <td>${escapeHtml(customer.address || "-")}</td>
                <td>
                    <div class="table-action-row">
                        <button class="table-button" type="button" onclick="startCustomerEdit(${customer.customerId})">Edit</button>
                        <button class="table-button danger" type="button" onclick="deleteCustomer(${customer.customerId})">Delete</button>
                    </div>
                </td>
            </tr>
        `).join("")
        : `<tr><td colspan="5"><div class="empty-state">No customers added yet.</div></td></tr>`;
}

function renderOrders() {
    refs.ordersList.innerHTML = state.orders.length
        ? state.orders.map((order) => `
            <article class="order-card">
                <div class="order-card-header">
                    <div>
                        <span class="stack-row-kicker">Order #${escapeHtml(String(order.orderId))}</span>
                        <h4>${escapeHtml(order.customerName || "Customer")}</h4>
                        <p class="stack-row-subtitle">${escapeHtml(formatDate(order.orderDate))}</p>
                    </div>
                    <strong>${escapeHtml(formatCurrency(order.totalAmount))}</strong>
                </div>
                <div class="order-items">
                    ${Array.isArray(order.items) && order.items.length
                        ? order.items.map((item) => `
                            <div class="order-item">
                                <strong>${escapeHtml(item.productName || "Product")}</strong>
                                <div class="stack-row-subtitle">Qty ${escapeHtml(String(item.quantity))} x ${escapeHtml(formatCurrency(item.price))} = ${escapeHtml(formatCurrency(item.lineTotal))}</div>
                            </div>
                        `).join("")
                        : `<div class="empty-state">No item details available.</div>`}
                </div>
            </article>
        `).join("")
        : `<div class="empty-state">Order history will appear here after you place the first order.</div>`;
}

async function handleProductSubmit(event) {
    event.preventDefault();

    if (state.user?.userRole !== "Admin") {
        showToast("Only Admin users can manage products.", "error");
        return;
    }

    const productId = Number(refs.productId.value);
    const isEdit = Boolean(productId);
    const payload = {
        productId,
        name: refs.productName.value.trim(),
        description: refs.productDescription.value.trim(),
        price: Number(refs.productPrice.value),
        quantity: Number(refs.productQuantity.value)
    };

    setButtonBusy(refs.productSubmitButton, true, isEdit ? "Saving..." : "Creating...");

    try {
        await apiRequest(isEdit ? `/product/${productId}` : "/product", {
            method: isEdit ? "PUT" : "POST",
            body: payload
        });

        resetProductForm();
        await refreshDashboard();
        showToast(isEdit ? "Product updated successfully." : "Product added successfully.", "success");
    } catch (error) {
        showToast(error.message || "Unable to save product.", "error");
    } finally {
        setButtonBusy(refs.productSubmitButton, false, "Save Product");
    }
}

async function handleCustomerSubmit(event) {
    event.preventDefault();

    const customerId = Number(refs.customerId.value);
    const isEdit = Boolean(customerId);
    const payload = {
        customerId,
        name: refs.customerName.value.trim(),
        email: refs.customerEmail.value.trim(),
        phone: refs.customerPhone.value.trim(),
        address: refs.customerAddress.value.trim()
    };

    const submitButton = refs.customerForm.querySelector('button[type="submit"]');
    setButtonBusy(submitButton, true, isEdit ? "Saving..." : "Creating...");

    try {
        await apiRequest(isEdit ? `/customer/${customerId}` : "/customer", {
            method: isEdit ? "PUT" : "POST",
            body: payload
        });

        resetCustomerForm();
        await refreshDashboard();
        showToast(isEdit ? "Customer updated successfully." : "Customer added successfully.", "success");
    } catch (error) {
        showToast(error.message || "Unable to save customer.", "error");
    } finally {
        setButtonBusy(submitButton, false, "Save Customer");
    }
}

async function handleOrderSubmit(event) {
    event.preventDefault();

    const customerId = Number(refs.orderCustomer.value);
    const rawItems = Array.from(refs.orderLines.querySelectorAll(".order-line")).map((row) => ({
        productId: Number(row.querySelector(".order-product").value),
        quantity: Number(row.querySelector(".order-quantity").value)
    }));

    const items = rawItems
        .filter((item) => item.productId > 0 && item.quantity > 0)
        .reduce((accumulator, item) => {
            const existing = accumulator.find((line) => line.productId === item.productId);
            if (existing) {
                existing.quantity += item.quantity;
            } else {
                accumulator.push({ ...item });
            }
            return accumulator;
        }, []);

    if (!customerId || items.length === 0) {
        showToast("Choose a customer and at least one valid order line.", "error");
        return;
    }

    const submitButton = refs.orderForm.querySelector('button[type="submit"]');
    setButtonBusy(submitButton, true, "Placing...");

    try {
        await apiRequest("/order", {
            method: "POST",
            body: {
                customerId,
                items
            }
        });

        refs.orderForm.reset();
        refs.orderLines.innerHTML = "";
        addOrderLine();
        await refreshDashboard();
        showToast("Order placed successfully.", "success");
    } catch (error) {
        showToast(error.message || "Unable to place order.", "error");
    } finally {
        setButtonBusy(submitButton, false, "Place Order");
    }
}

function populateOrderCustomerOptions() {
    refs.orderCustomer.innerHTML = state.customers.length
        ? ['<option value="">Select customer</option>']
            .concat(state.customers.map((customer) => `<option value="${customer.customerId}">${escapeHtml(customer.name)}</option>`))
            .join("")
        : '<option value="">Add a customer first</option>';
}

function rebuildOrderLines() {
    if (!refs.orderLines.children.length) {
        addOrderLine();
        return;
    }

    Array.from(refs.orderLines.querySelectorAll(".order-product")).forEach((select) => {
        const selectedValue = select.value;
        select.innerHTML = buildProductOptions();
        select.value = selectedValue;
    });
}

function addOrderLine(defaults = {}) {
    const row = document.createElement("div");
    row.className = "order-line";

    row.innerHTML = `
        <label class="field">
            <span>Product</span>
            <select class="order-product">${buildProductOptions()}</select>
        </label>
        <label class="field">
            <span>Quantity</span>
            <input class="order-quantity" type="number" min="1" step="1" value="${escapeHtml(String(defaults.quantity || 1))}">
        </label>
        <button class="icon-button" type="button" title="Remove line">X</button>
    `;

    const productSelect = row.querySelector(".order-product");
    const quantityInput = row.querySelector(".order-quantity");
    const removeButton = row.querySelector(".icon-button");

    if (defaults.productId) {
        productSelect.value = String(defaults.productId);
    }

    productSelect.addEventListener("change", updateOrderTotal);
    quantityInput.addEventListener("input", updateOrderTotal);
    removeButton.addEventListener("click", () => {
        row.remove();
        if (!refs.orderLines.children.length) {
            addOrderLine();
        }
        updateOrderTotal();
    });

    refs.orderLines.appendChild(row);
    updateOrderTotal();
}

function buildProductOptions() {
    if (!state.products.length) {
        return '<option value="">Add a product first</option>';
    }

    return ['<option value="">Select product</option>']
        .concat(state.products.map((product) => `
            <option value="${product.productId}">
                ${escapeHtml(product.name)} (${escapeHtml(String(product.quantity))} in stock)
            </option>
        `))
        .join("");
}

function updateOrderTotal() {
    const total = Array.from(refs.orderLines.querySelectorAll(".order-line")).reduce((sum, row) => {
        const productId = Number(row.querySelector(".order-product").value);
        const quantity = Number(row.querySelector(".order-quantity").value);
        const product = state.products.find((item) => item.productId === productId);

        if (!product || quantity <= 0) {
            return sum;
        }

        return sum + Number(product.price) * quantity;
    }, 0);

    refs.orderTotalValue.textContent = formatCurrency(total);
}

function resetProductForm() {
    state.editingProductId = null;
    refs.productForm.reset();
    refs.productId.value = "";
    refs.productFormHeading.textContent = "Add product";
    refs.productCancelButton.classList.add("hidden");
}

function resetCustomerForm() {
    state.editingCustomerId = null;
    refs.customerForm.reset();
    refs.customerId.value = "";
    refs.customerFormHeading.textContent = "Add customer";
    refs.customerCancelButton.classList.add("hidden");
}

function startProductEdit(productId) {
    const product = state.products.find((item) => item.productId === productId);
    if (!product) {
        return;
    }

    state.editingProductId = productId;
    refs.productId.value = product.productId;
    refs.productName.value = product.name || "";
    refs.productDescription.value = product.description || "";
    refs.productPrice.value = product.price;
    refs.productQuantity.value = product.quantity;
    refs.productFormHeading.textContent = `Edit ${product.name}`;
    refs.productCancelButton.classList.remove("hidden");
    document.getElementById("productsSection")?.scrollIntoView({ behavior: "smooth", block: "start" });
}

function startCustomerEdit(customerId) {
    const customer = state.customers.find((item) => item.customerId === customerId);
    if (!customer) {
        return;
    }

    state.editingCustomerId = customerId;
    refs.customerId.value = customer.customerId;
    refs.customerName.value = customer.name || "";
    refs.customerEmail.value = customer.email || "";
    refs.customerPhone.value = customer.phone || "";
    refs.customerAddress.value = customer.address || "";
    refs.customerFormHeading.textContent = `Edit ${customer.name}`;
    refs.customerCancelButton.classList.remove("hidden");
    document.getElementById("customersSection")?.scrollIntoView({ behavior: "smooth", block: "start" });
}

async function deleteProduct(productId) {
    if (!window.confirm("Delete this product?")) {
        return;
    }

    try {
        await apiRequest(`/product/${productId}`, { method: "DELETE" });
        await refreshDashboard();
        showToast("Product deleted successfully.", "success");
    } catch (error) {
        showToast(error.message || "Unable to delete product.", "error");
    }
}

async function deleteCustomer(customerId) {
    if (!window.confirm("Delete this customer?")) {
        return;
    }

    try {
        await apiRequest(`/customer/${customerId}`, { method: "DELETE" });
        await refreshDashboard();
        showToast("Customer deleted successfully.", "success");
    } catch (error) {
        showToast(error.message || "Unable to delete customer.", "error");
    }
}

function logout(showMessage = true) {
    state.token = "";
    state.user = null;
    state.products = [];
    state.customers = [];
    state.orders = [];
    localStorage.removeItem("inventoryToken");
    showAuthView();
    refs.authForm.reset();
    refs.userRole.value = "Admin";
    switchAuthMode("login");
    resetProductForm();
    resetCustomerForm();
    refs.orderLines.innerHTML = "";

    if (showMessage) {
        showToast("You have been logged out.", "success");
    }
}

async function apiRequest(path, options = {}) {
    const config = {
        method: options.method || "GET",
        headers: {},
        authorize: options.authorize !== false
    };

    if (options.body !== undefined) {
        config.headers["Content-Type"] = "application/json";
        config.body = JSON.stringify(options.body);
    }

    if (config.authorize && state.token) {
        config.headers.Authorization = `Bearer ${state.token}`;
    }

    const response = await fetch(`${apiBase}${path}`, config);
    const contentType = response.headers.get("content-type") || "";
    const payload = contentType.includes("application/json")
        ? await response.json()
        : await response.text();

    if (!response.ok) {
        if (response.status === 401) {
            logout(false);
        }

        const message = typeof payload === "string"
            ? payload
            : payload?.detail || payload?.title || readValidationMessage(payload) || "Request failed.";

        throw new Error(message);
    }

    return payload;
}

function readValidationMessage(payload) {
    if (!payload?.errors) {
        return "";
    }

    const firstErrorGroup = Object.values(payload.errors)[0];
    return Array.isArray(firstErrorGroup) ? firstErrorGroup[0] : "";
}

function setButtonBusy(button, busy, busyText) {
    if (!button) {
        return;
    }

    if (!button.dataset.defaultText) {
        button.dataset.defaultText = button.textContent;
    }

    button.disabled = busy;
    button.textContent = busy ? busyText : button.dataset.defaultText;
}

function showToast(message, type = "success") {
    window.clearTimeout(toastTimer);
    refs.toast.textContent = message;
    refs.toast.className = `toast ${type}`;

    toastTimer = window.setTimeout(() => {
        refs.toast.className = "toast hidden";
    }, 3200);
}

function formatCurrency(value) {
    const amount = Number(value || 0);
    return `Rs. ${amount.toFixed(2)}`;
}

function formatDate(value) {
    const date = new Date(value);
    return Number.isNaN(date.getTime()) ? "-" : date.toLocaleString("en-IN", {
        day: "2-digit",
        month: "short",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit"
    });
}

function getStockLabel(quantity) {
    if (quantity <= 0) {
        return "Out of stock";
    }

    if (quantity <= 10) {
        return "Low stock";
    }

    return "Healthy";
}

function getStockClass(quantity) {
    if (quantity <= 0) {
        return "empty";
    }

    if (quantity <= 10) {
        return "warn";
    }

    return "ok";
}

function escapeHtml(value) {
    return String(value ?? "")
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#39;");
}

window.startProductEdit = startProductEdit;
window.startCustomerEdit = startCustomerEdit;
window.deleteProduct = deleteProduct;
window.deleteCustomer = deleteCustomer;
