// kanban.js - Drag and drop utilities
window.kanbanInterop = {
    // Create and show the insertion indicator
    showInsertionLine: function(columnId, mouseY) {
        // Remove any existing indicators
        this.removeInsertionLine();

        // Find the column container
        const column = document.querySelector(`.column-${columnId} .cards-container`);
        if (!column) return null;

        // Find all cards in the column
        const cards = column.querySelectorAll('.kanban-card');
        if (!cards.length) {
            // Empty column - just add indicator at the top
            this.createIndicator(column, true);
            return 0;
        }

        // Find the insertion point based on mouse Y position
        let insertBefore = null;
        let insertIndex = cards.length; // Default to end

        for (let i = 0; i < cards.length; i++) {
            const card = cards[i];
            const rect = card.getBoundingClientRect();
            const cardMiddle = rect.top + rect.height / 2;

            if (mouseY < cardMiddle) {
                insertBefore = card;
                insertIndex = i;
                break;
            }
        }

        // Create the indicator
        if (insertBefore) {
            this.createIndicator(column, false, insertBefore);
        } else {
            // Append at the end
            this.createIndicator(column, false);
        }

        return insertIndex;
    },

    // Create the indicator element
    createIndicator: function(container, isEmpty, beforeElement = null) {
        const indicator = document.createElement('div');
        indicator.id = 'insertion-indicator';
        indicator.style.height = '4px';
        indicator.style.backgroundColor = 'var(--column-ready-color)';
        indicator.style.margin = '8px 0';
        indicator.style.borderRadius = '2px';
        indicator.style.transition = 'all 0.1s ease';

        if (isEmpty) {
            // Empty column gets a bigger indicator
            indicator.style.height = '8px';
            container.appendChild(indicator);
        } else if (beforeElement) {
            container.insertBefore(indicator, beforeElement);
        } else {
            container.appendChild(indicator);
        }

        // Animate in
        setTimeout(() => {
            indicator.style.height = isEmpty ? '8px' : '4px';
            indicator.style.boxShadow = '0 0 8px var(--column-ready-color)';
        }, 0);
    },

    // Remove the insertion indicator
    removeInsertionLine: function() {
        const indicator = document.getElementById('insertion-indicator');
        if (indicator) {
            indicator.remove();
        }
    },

    // Get the position value for insertion
    calculatePosition: function(columnId, insertIndex) {
        // This function needs to be implemented on the C# side
        // as it requires access to the task data model
        return DotNet.invokeMethodAsync('LifeKanban', 'CalculatePositionValue', columnId, insertIndex);
    }
};