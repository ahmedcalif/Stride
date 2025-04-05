$(document).ready(function () {
  // Add SPA link handling to all navigation links
  initializeSpaNavigation();

  // Initialize browser history handling
  initializeHistoryHandling();

  // Initialize active menu highlighting
  highlightActiveMenu();

  // Initialize form validation
  initializeClientValidation();
});

// Add click handlers to all links with spa-link class
function initializeSpaNavigation() {
  $(document).on("click", ".spa-link", function (e) {
    e.preventDefault();

    const url = $(this).attr("href");
    navigateTo(url);
  });
}

// Handle browser back/forward buttons
function initializeHistoryHandling() {
  window.addEventListener("popstate", function (e) {
    if (e.state && e.state.url) {
      loadContent(e.state.url, false);
      highlightActiveMenu(e.state.url);
    }
  });

  // Initialize history state for the current page
  const currentPath = window.location.pathname;
  history.replaceState({ url: currentPath }, "", currentPath);
}

// Navigate to a new URL within the SPA
function navigateTo(url) {
  // Update browser history
  history.pushState({ url: url }, "", url);

  // Load the content
  loadContent(url);

  // Update active menu
  highlightActiveMenu(url);
}

// Load content via AJAX
function loadContent(url, showLoader = true) {
  if (showLoader) {
    // Show loading indicator
    $("#content-container").html(
      '<div class="text-center my-5"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>'
    );
  }

  $.ajax({
    url: "/Spa/LoadContent",
    type: "GET",
    data: { path: url },
    success: function (response) {
      $("#content-container").html(response);

      // Re-initialize client validation
      initializeClientValidation();

      // Scroll to top
      window.scrollTo(0, 0);
    },
    error: function (xhr) {
      if (xhr.status === 401) {
        // Unauthorized - redirect to login page
        window.location.href = "/Identity/Account/Login";
      } else if (xhr.status === 403) {
        // Forbidden
        $("#content-container").html(`
                    <div class="container text-center my-5">
                        <div class="alert alert-danger">
                            <h1><i class="fas fa-exclamation-triangle"></i> Access Denied</h1>
                            <p>You don't have permission to access this page.</p>
                        </div>
                        <a href="/Dashboard" class="btn btn-primary spa-link">Back to Dashboard</a>
                    </div>
                `);
      } else {
        // Show error page for other errors
        $("#content-container").html(`
                    <div class="container text-center my-5">
                        <div class="alert alert-danger">
                            <h1><i class="fas fa-exclamation-circle"></i> Error</h1>
                            <p>An error occurred while loading the page. Please try again.</p>
                        </div>
                        <a href="/Dashboard" class="btn btn-primary spa-link">Back to Dashboard</a>
                    </div>
                `);
      }
    },
  });
}
function highlightActiveMenu(url) {
  url = url || window.location.pathname;

  // Remove active class from all navigation links
  $(".nav-link").removeClass("text-primary").addClass("text-secondary");

  // Add active class to the matching link
  $(`.nav-link[href="${url}"]`)
    .removeClass("text-secondary")
    .addClass("text-primary");

  // If no exact match, try to match the controller
  if (!$(`.nav-link[href="${url}"]`).length) {
    const controller = url.split("/")[1]; // Extract controller name from URL
    if (controller) {
      $(`.nav-link[href="/${controller}"]`)
        .removeClass("text-secondary")
        .addClass("text-primary");
    }
  }
}

// Initialize client validation for forms
function initializeClientValidation() {
  if ($.validator && $.validator.unobtrusive) {
    $.validator.unobtrusive.parse("form");
  }
}

// Function to show alerts
function showAlert(type, message) {
  const alertHtml = `
        <div class="alert alert-${type} alert-dismissible fade show" role="alert">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;

  // Insert alert at the top of the content
  $("#content-container").prepend(alertHtml);

  // Auto-dismiss after 5 seconds
  setTimeout(function () {
    $(".alert").alert("close");
  }, 5000);
}
