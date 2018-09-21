using System;
using Microsoft.EntityFrameworkCore;

namespace DynamicModel {
    class DynamicContext : DbContext {
        private readonly ModelOptions modelOptions;
        public DynamicContext(DbContextOptions options, ModelOptions modelOptions) : base(options) {
            this.modelOptions = modelOptions;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity(modelOptions.ModelType);
        }
    }
}
