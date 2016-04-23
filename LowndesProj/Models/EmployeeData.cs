namespace LowndesProj.Models {
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class EmployeeData : DbContext {
        public EmployeeData()
            : base( "name=EmployeeData" ) {
        }

        public virtual DbSet<EOM_Nomination> EOM_Nomination { get; set; }
        public virtual DbSet<UltiPro> UltiPro { get; set; }

        protected override void OnModelCreating( DbModelBuilder modelBuilder ) {
            modelBuilder.Entity<EOM_Nomination>()
                .Property( e => e.Nominator_Employee_Number )
                .IsUnicode( false );

            modelBuilder.Entity<EOM_Nomination>()
                .Property( e => e.Nominator_Employee_Full_Name )
                .IsUnicode( false );

            modelBuilder.Entity<EOM_Nomination>()
                .Property( e => e.Nomination_Reason )
                .IsUnicode( false );

            modelBuilder.Entity<EOM_Nomination>()
                .Property( e => e.Nominee_Employee_Number )
                .IsUnicode( false );

            modelBuilder.Entity<EOM_Nomination>()
                .Property( e => e.Nominee_Emp_or_Team_Name )
                .IsUnicode( false );

            modelBuilder.Entity<UltiPro>()
                .Property( e => e.employment_status )
                .IsUnicode( false );

            modelBuilder.Entity<UltiPro>()
                .Property( e => e.employee_number )
                .IsUnicode( false );

            modelBuilder.Entity<UltiPro>()
                .Property( e => e.last_name )
                .IsUnicode( false );

            modelBuilder.Entity<UltiPro>()
                .Property( e => e.first_name )
                .IsUnicode( false );

            modelBuilder.Entity<UltiPro>()
                .Property( e => e.middle_name )
                .IsUnicode( false );

            modelBuilder.Entity<UltiPro>()
                .Property( e => e.preferred_first_name )
                .IsUnicode( false );

            modelBuilder.Entity<UltiPro>()
                .Property( e => e.org_level_1 )
                .IsUnicode( false );

            modelBuilder.Entity<UltiPro>()
                .Property( e => e.org_level_2 )
                .IsUnicode( false );

            modelBuilder.Entity<UltiPro>()
                .Property( e => e.org_level_3 )
                .IsUnicode( false );

            modelBuilder.Entity<UltiPro>()
                .Property( e => e.location_code )
                .IsUnicode( false );

            modelBuilder.Entity<UltiPro>()
                .Property( e => e.gender_code )
                .IsUnicode( false );

            modelBuilder.Entity<UltiPro>()
                .Property( e => e.job )
                .IsUnicode( false );

            modelBuilder.Entity<UltiPro>()
                .Property( e => e.supervisor_name_last_suffix_first_mi )
                .IsUnicode( false );

            modelBuilder.Entity<UltiPro>()
                .Property( e => e.supervisor_employee_number )
                .IsUnicode( false );

            modelBuilder.Entity<UltiPro>()
                .Property( e => e.eecudfield01 )
                .IsUnicode( false );

            modelBuilder.Entity<UltiPro>()
                .Property( e => e.work_extension )
                .IsUnicode( false );

            modelBuilder.Entity<UltiPro>()
                .Property( e => e.work_phone )
                .IsUnicode( false );
        }
    }
}
