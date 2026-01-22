// Configuration variable - set default state (true = expanded, false = collapsed)
const DEFAULT_EXPAND_STATE = true; // Change to true if you want expanded by default

document.addEventListener('DOMContentLoaded', async function () {
    console.log('Starting to read OpenAPI JSON...');

    try {
        const response = await fetch('/doc_json/swagger.json');

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const openApiData = await response.json();
        displayPaths(openApiData.paths);

    } catch (error) {
        console.error('Error reading JSON file:', error);
        displayError(error.message);
    }
});

function displayPaths(paths) {
    const container = document.getElementById('api-paths') || createContainer('api-paths', '');

    if (!paths) {
        container.innerHTML += '<p>No paths found in the API specification.</p>';
        return;
    }

    // Header with title and buttons on the same line
    const headerHtml = `
        <div class="paths-header" style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
            <h2 style="margin: 0;">API Endpoints</h2>
            <div class="controls" style="display: flex; gap: 0.5rem;">
                <button id="expand-all" class="btn btn-primary btn-sm">Expand All</button>
                <button id="collapse-all" class="btn btn-secondary btn-sm">Collapse All</button>
            </div>
        </div>
    `;

    let pathsHtml = headerHtml + '<div class="paths-container">';

    Object.entries(paths).forEach(([path, methods]) => {
        pathsHtml += createPathCard(path, methods);
    });

    pathsHtml += '</div>';
    container.innerHTML = pathsHtml;

    // Initialize all method sections based on default state
    initializeAllSections();

    // Add event listeners for expand/collapse buttons
    document.getElementById('expand-all').addEventListener('click', expandAllSections);
    document.getElementById('collapse-all').addEventListener('click', collapseAllSections);

    // Add individual click handlers
    document.querySelectorAll('.method-header').forEach(header => {
        header.addEventListener('click', toggleSingleSection);
    });
}

function createPathCard(path, methods) {
    let methodsHtml = '';

    Object.entries(methods).forEach(([method, details]) => {
        const methodUpper = method.toUpperCase();
        let methodBadgeClass = 'method-badge';

        // Color code based on HTTP method
        switch (methodUpper) {
            case 'GET': methodBadgeClass += ' get'; break;
            case 'POST': methodBadgeClass += ' post'; break;
            case 'PUT': methodBadgeClass += ' put'; break;
            case 'PATCH': methodBadgeClass += ' patch'; break;
            case 'DELETE': methodBadgeClass += ' delete'; break;
            default: methodBadgeClass += ' default';
        }

        // Set arrow based on default state
        const arrow = DEFAULT_EXPAND_STATE ? '▼' : '▶';

        methodsHtml += `
            <div class="method-section">
                <div class="method-header">
                    <span class="toggle-arrow">${arrow}</span>
                    <span class="${methodBadgeClass}">${methodUpper}</span>
                    <span class="path-url">${path}</span>
                    <span class="method-summary">${details.summary || 'No summary available'}</span>
                </div>
                <div class="method-details">
                    ${details.parameters ? `
                        <div class="parameters-section">
                            <strong>Parameters:</strong>
                            <ul>
                                ${details.parameters.map(param => `
                                    <li>
                                        <span class="param-name"><code>${param.name}</code></span>
                                        <span class="param-location">(${param.in})</span>
                                        - <span class="param-required">${param.required ? 'Required' : 'Optional'}</span>
                                        ${param.schema ? `<span class="param-type">${param.schema.type || 'N/A'}</span>` : ''}
                                        ${param.description ? `<br><small class="param-desc">${param.description}</small>` : ''}
                                    </li>
                                `).join('')}
                            </ul>
                        </div>
                    ` : ''}
                    
                    ${details.requestBody ? `
                        <div class="request-body-section">
                            <strong>Request Body:</strong>
                            ${details.requestBody.required ? '<span class="required-badge">Required</span>' : ''}
                            ${details.requestBody.description ? `<p class="request-desc">${details.requestBody.description}</p>` : ''}
                            
                            ${details.requestBody.content && details.requestBody.content['application/json'] ? `
                                ${formatRequestBody(details.requestBody.content['application/json'])}
                            ` : ''}
                        </div>
                    ` : ''}
                    
                    ${methodUpper === 'GET' || methodUpper === 'POST' || methodUpper === 'PUT' || methodUpper === 'DELETE' ? `
                        <div class="authentication-section">
                            <strong>Authentication:</strong>
                            <p>This endpoint requires Bearer token authentication.</p>
                            <div class="auth-example">
                                <strong>Example Authorization Header:</strong>
                                <div class="example-value">
                                    <pre><code class="language-text">Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzd...</code></pre>
                                </div>
                            </div>
                        </div>
                    ` : ''}
                    
                    <div class="responses-section">
                        <strong>Responses:</strong>
                        <ul class="responses-list">
                            ${Object.entries(details.responses || {}).map(([code, response]) => `
                                <li class="response-item">
                                    <div class="response-header">
                                        <span class="status-code status-${code}">${code}</span>
                                        <span class="response-description">${response.description || 'No description'}</span>
                                    </div>
                                    ${formatResponseExamples(response) || ''}
                                </li>
                            `).join('')}
                        </ul>
                    </div>
                </div>
            </div>
        `;
    });

    return `
    <div class="path-card">
        ${methodsHtml}
    </div>
`;
}

// NEW FUNCTIONS FOR EXPAND/COLLAPSE CONTROLS

function initializeAllSections() {
    document.querySelectorAll('.method-header').forEach(header => {
        const details = header.nextElementSibling;
        const arrow = header.querySelector('.toggle-arrow');

        if (DEFAULT_EXPAND_STATE) {
            details.style.display = 'block';
            arrow.textContent = '▼';
        } else {
            details.style.display = 'none';
            arrow.textContent = '▶';
        }
    });
}

function toggleSingleSection(event) {
    const header = event.currentTarget;
    const details = header.nextElementSibling;
    const arrow = header.querySelector('.toggle-arrow');

    if (details.style.display === 'none') {
        details.style.display = 'block';
        arrow.textContent = '▼';
    } else {
        details.style.display = 'none';
        arrow.textContent = '▶';
    }
}

function expandAllSections() {
    document.querySelectorAll('.method-header').forEach(header => {
        const details = header.nextElementSibling;
        const arrow = header.querySelector('.toggle-arrow');

        details.style.display = 'block';
        arrow.textContent = '▼';
    });
}

function collapseAllSections() {
    document.querySelectorAll('.method-header').forEach(header => {
        const details = header.nextElementSibling;
        const arrow = header.querySelector('.toggle-arrow');

        details.style.display = 'none';
        arrow.textContent = '▶';
    });
}

// Format request body with example only
function formatRequestBody(content) {
    let requestBodyHtml = '';

    if (content.example) {
        requestBodyHtml += `
            <div class="request-example">
                <strong>Example Request Body:</strong>
                <div class="example-value">
                    <pre><code class="language-json">${JSON.stringify(content.example, null, 2)}</code></pre>
                </div>
            </div>
        `;
    }

    return requestBodyHtml;
}

// Format response examples
function formatResponseExamples(response) {
    let examplesHtml = '';

    if (response.content && response.content['application/json']) {
        const content = response.content['application/json'];

        if (content.examples) {
            examplesHtml = `
                <div class="examples-container">
                    <strong>Examples:</strong>
                    ${Object.entries(content.examples).map(([exampleName, example]) => `
                        <div class="example-item">
                            <div class="example-header">
                                <span class="example-name">${exampleName}</span>
                                ${example.summary ? `<span class="example-summary">- ${example.summary}</span>` : ''}
                            </div>
                            ${example.value ? `
                                <div class="example-value">
                                    <pre><code class="language-json">${JSON.stringify(example.value, null, 2)}</code></pre>
                                </div>
                            ` : ''}
                        </div>
                    `).join('')}
                </div>
            `;
        }
        else if (content.example) {
            examplesHtml = `
                <div class="examples-container">
                    <strong>Example:</strong>
                    <div class="example-item">
                        <div class="example-value">
                            <pre><code class="language-json">${JSON.stringify(content.example, null, 2)}</code></pre>
                        </div>
                    </div>
                </div>
            `;
        }
    }

    return examplesHtml;
}

function createContainer(id, title) {
    const container = document.createElement('div');
    container.id = id;
    container.className = 'api-section';

    const titleElement = document.createElement('h2');
    titleElement.textContent = title;
    container.appendChild(titleElement);

    const mainContent = document.querySelector('main') || document.body;
    mainContent.appendChild(container);

    return container;
}

function displayError(message) {
    const errorDiv = document.createElement('div');
    errorDiv.className = 'error-message';
    errorDiv.innerHTML = `
        <h3>Error Loading API Documentation</h3>
        <p>${message}</p>
        <p>Make sure the OpenAPI JSON file is accessible at <code>/openapi.json</code></p>
    `;

    const mainContent = document.querySelector('main') || document.body;
    mainContent.appendChild(errorDiv);
}