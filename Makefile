PLUGIN_PREFIX := StringTemplates.Plugins.
PLUGINS := $(patsubst src/plugins/$(PLUGIN_PREFIX)%,%,$(wildcard src/plugins/$(PLUGIN_PREFIX)*))

.PHONY: all core plugins $(PLUGINS) list

all: core plugins

core:
	./build-core.sh

plugins: $(PLUGINS)

$(PLUGINS):
	./build-plugin.sh $@

list:
	@echo "core"
	@for p in $(PLUGINS); do echo "$$p"; done
