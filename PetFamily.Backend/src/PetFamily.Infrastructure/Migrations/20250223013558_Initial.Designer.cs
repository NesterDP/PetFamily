﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PetFamily.Infrastructure;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250223013558_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PetFamily.Domain.PetContext.Entities.Pet", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                        .HasColumnName("creation_date");

                    b.Property<bool>("_isDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<Guid?>("volunteer_id")
                        .HasColumnType("uuid")
                        .HasColumnName("volunteer_id");

                    b.ComplexProperty<Dictionary<string, object>>("Address", "PetFamily.Domain.PetContext.Entities.Pet.Address#Address", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Apartment")
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("apartment");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("city");

                            b1.Property<string>("House")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("house");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Color", "PetFamily.Domain.PetContext.Entities.Pet.Color#Color", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)")
                                .HasColumnName("color");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("DateOfBirth", "PetFamily.Domain.PetContext.Entities.Pet.DateOfBirth#DateOfBirth", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<DateTime>("Value")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("date_of_birth");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Description", "PetFamily.Domain.PetContext.Entities.Pet.Description#Description", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(2000)
                                .HasColumnType("character varying(2000)")
                                .HasColumnName("description");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("HealthInfo", "PetFamily.Domain.PetContext.Entities.Pet.HealthInfo#HealthInfo", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(800)
                                .HasColumnType("character varying(800)")
                                .HasColumnName("health_info");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Height", "PetFamily.Domain.PetContext.Entities.Pet.Height#Height", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<float>("Value")
                                .HasColumnType("real")
                                .HasColumnName("height_info");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("HelpStatus", "PetFamily.Domain.PetContext.Entities.Pet.HelpStatus#HelpStatus", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<int>("Value")
                                .HasColumnType("integer")
                                .HasColumnName("help_status");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("IsCastrated", "PetFamily.Domain.PetContext.Entities.Pet.IsCastrated#IsCastrated", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<bool>("Value")
                                .HasColumnType("boolean")
                                .HasColumnName("is_castrated");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("IsVaccinated", "PetFamily.Domain.PetContext.Entities.Pet.IsVaccinated#IsVaccinated", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<bool>("Value")
                                .HasColumnType("boolean")
                                .HasColumnName("is_vaccinated");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Name", "PetFamily.Domain.PetContext.Entities.Pet.Name#Name", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("name");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("OwnerPhoneNumber", "PetFamily.Domain.PetContext.Entities.Pet.OwnerPhoneNumber#Phone", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(20)
                                .HasColumnType("character varying(20)")
                                .HasColumnName("owner_phone");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("PetClassification", "PetFamily.Domain.PetContext.Entities.Pet.PetClassification#PetClassification", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<Guid>("BreedId")
                                .HasColumnType("uuid")
                                .HasColumnName("breed_id");

                            b1.Property<Guid>("SpeciesId")
                                .HasColumnType("uuid")
                                .HasColumnName("species_id");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Position", "PetFamily.Domain.PetContext.Entities.Pet.Position#Position", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<int>("Value")
                                .HasColumnType("integer")
                                .HasColumnName("position");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Weight", "PetFamily.Domain.PetContext.Entities.Pet.Weight#Weight", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<float>("Value")
                                .HasColumnType("real")
                                .HasColumnName("weight_info");
                        });

                    b.HasKey("Id")
                        .HasName("pk_pets");

                    b.HasIndex("volunteer_id")
                        .HasDatabaseName("ix_pets_volunteer_id");

                    b.ToTable("pets", (string)null);
                });

            modelBuilder.Entity("PetFamily.Domain.PetContext.Entities.Volunteer", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<bool>("_isDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.ComplexProperty<Dictionary<string, object>>("Description", "PetFamily.Domain.PetContext.Entities.Volunteer.Description#Description", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(2000)
                                .HasColumnType("character varying(2000)")
                                .HasColumnName("description");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Email", "PetFamily.Domain.PetContext.Entities.Volunteer.Email#Email", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)")
                                .HasColumnName("email");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Experience", "PetFamily.Domain.PetContext.Entities.Volunteer.Experience#Experience", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<int>("Value")
                                .HasColumnType("integer")
                                .HasColumnName("experience");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("FullName", "PetFamily.Domain.PetContext.Entities.Volunteer.FullName#FullName", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("FirstName")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("first_name");

                            b1.Property<string>("LastName")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("last_name");

                            b1.Property<string>("Surname")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("surname");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("PhoneNumber", "PetFamily.Domain.PetContext.Entities.Volunteer.PhoneNumber#Phone", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(20)
                                .HasColumnType("character varying(20)")
                                .HasColumnName("phone_number");
                        });

                    b.HasKey("Id")
                        .HasName("pk_volunteers");

                    b.ToTable("volunteers", (string)null);
                });

            modelBuilder.Entity("PetFamily.Domain.SpeciesContext.Entities.Breed", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid?>("species_id")
                        .HasColumnType("uuid")
                        .HasColumnName("species_id");

                    b.ComplexProperty<Dictionary<string, object>>("Name", "PetFamily.Domain.SpeciesContext.Entities.Breed.Name#Name", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("name");
                        });

                    b.HasKey("Id")
                        .HasName("pk_breeds");

                    b.HasIndex("species_id")
                        .HasDatabaseName("ix_breeds_species_id");

                    b.ToTable("breeds", (string)null);
                });

            modelBuilder.Entity("PetFamily.Domain.SpeciesContext.Entities.Species", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.ComplexProperty<Dictionary<string, object>>("Name", "PetFamily.Domain.SpeciesContext.Entities.Species.Name#Name", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("name");
                        });

                    b.HasKey("Id")
                        .HasName("pk_species");

                    b.ToTable("species", (string)null);
                });

            modelBuilder.Entity("PetFamily.Domain.PetContext.Entities.Pet", b =>
                {
                    b.HasOne("PetFamily.Domain.PetContext.Entities.Volunteer", null)
                        .WithMany("AllOwnedPets")
                        .HasForeignKey("volunteer_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_pets_volunteers_volunteer_id");

                    b.OwnsOne("PetFamily.Domain.Shared.SharedVO.TransferDetailsList", "TransferDetailsList", b1 =>
                        {
                            b1.Property<Guid>("PetId")
                                .HasColumnType("uuid");

                            b1.HasKey("PetId")
                                .HasName("pk_pets");

                            b1.ToTable("pets");

                            b1.ToJson("transfer_details_list");

                            b1.WithOwner()
                                .HasForeignKey("PetId")
                                .HasConstraintName("fk_pets_pets_pet_id");

                            b1.OwnsMany("PetFamily.Domain.Shared.SharedVO.TransferDetail", "TransferDetails", b2 =>
                                {
                                    b2.Property<Guid>("TransferDetailsListPetId")
                                        .HasColumnType("uuid");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("integer");

                                    b2.Property<string>("Description")
                                        .IsRequired()
                                        .HasMaxLength(2000)
                                        .HasColumnType("character varying(2000)");

                                    b2.Property<string>("Name")
                                        .IsRequired()
                                        .HasMaxLength(50)
                                        .HasColumnType("character varying(50)");

                                    b2.HasKey("TransferDetailsListPetId", "Id")
                                        .HasName("pk_pets");

                                    b2.ToTable("pets");

                                    b2.WithOwner()
                                        .HasForeignKey("TransferDetailsListPetId")
                                        .HasConstraintName("fk_pets_pets_transfer_details_list_pet_id");
                                });

                            b1.Navigation("TransferDetails");
                        });

                    b.OwnsOne("PetFamily.Domain.Shared.SharedVO.PhotosList", "PhotosList", b1 =>
                        {
                            b1.Property<Guid>("PetId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.HasKey("PetId");

                            b1.ToTable("pets");

                            b1.ToJson("photos_list");

                            b1.WithOwner()
                                .HasForeignKey("PetId")
                                .HasConstraintName("fk_pets_pets_id");

                            b1.OwnsMany("PetFamily.Domain.Shared.SharedVO.Photo", "Photos", b2 =>
                                {
                                    b2.Property<Guid>("PhotosListPetId")
                                        .HasColumnType("uuid");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("integer");

                                    b2.Property<string>("PathToStorage")
                                        .IsRequired()
                                        .HasMaxLength(50)
                                        .HasColumnType("character varying(50)");

                                    b2.HasKey("PhotosListPetId", "Id")
                                        .HasName("pk_pets");

                                    b2.ToTable("pets");

                                    b2.WithOwner()
                                        .HasForeignKey("PhotosListPetId")
                                        .HasConstraintName("fk_pets_pets_photos_list_pet_id");
                                });

                            b1.Navigation("Photos");
                        });

                    b.Navigation("PhotosList")
                        .IsRequired();

                    b.Navigation("TransferDetailsList")
                        .IsRequired();
                });

            modelBuilder.Entity("PetFamily.Domain.PetContext.Entities.Volunteer", b =>
                {
                    b.OwnsOne("PetFamily.Domain.Shared.SharedVO.TransferDetailsList", "TransferDetailsList", b1 =>
                        {
                            b1.Property<Guid>("VolunteerId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.HasKey("VolunteerId");

                            b1.ToTable("volunteers");

                            b1.ToJson("transfer_details_list");

                            b1.WithOwner()
                                .HasForeignKey("VolunteerId")
                                .HasConstraintName("fk_volunteers_volunteers_id");

                            b1.OwnsMany("PetFamily.Domain.Shared.SharedVO.TransferDetail", "TransferDetails", b2 =>
                                {
                                    b2.Property<Guid>("TransferDetailsListVolunteerId")
                                        .HasColumnType("uuid");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("integer");

                                    b2.Property<string>("Description")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.Property<string>("Name")
                                        .IsRequired()
                                        .HasColumnType("text");

                                    b2.HasKey("TransferDetailsListVolunteerId", "Id")
                                        .HasName("pk_volunteers");

                                    b2.ToTable("volunteers");

                                    b2.WithOwner()
                                        .HasForeignKey("TransferDetailsListVolunteerId")
                                        .HasConstraintName("fk_volunteers_volunteers_transfer_details_list_volunteer_id");
                                });

                            b1.Navigation("TransferDetails");
                        });

                    b.OwnsOne("PetFamily.Domain.PetContext.ValueObjects.VolunteerVO.SocialNetworksList", "SocialNetworksList", b1 =>
                        {
                            b1.Property<Guid>("VolunteerId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.HasKey("VolunteerId");

                            b1.ToTable("volunteers");

                            b1.ToJson("social_networks_list");

                            b1.WithOwner()
                                .HasForeignKey("VolunteerId")
                                .HasConstraintName("fk_volunteers_volunteers_id");

                            b1.OwnsMany("PetFamily.Domain.PetContext.ValueObjects.VolunteerVO.SocialNetwork", "SocialNetworks", b2 =>
                                {
                                    b2.Property<Guid>("SocialNetworksListVolunteerId")
                                        .HasColumnType("uuid");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("integer");

                                    b2.Property<string>("Link")
                                        .IsRequired()
                                        .HasMaxLength(100)
                                        .HasColumnType("character varying(100)");

                                    b2.Property<string>("Name")
                                        .IsRequired()
                                        .HasMaxLength(50)
                                        .HasColumnType("character varying(50)");

                                    b2.HasKey("SocialNetworksListVolunteerId", "Id")
                                        .HasName("pk_volunteers");

                                    b2.ToTable("volunteers");

                                    b2.WithOwner()
                                        .HasForeignKey("SocialNetworksListVolunteerId")
                                        .HasConstraintName("fk_volunteers_volunteers_social_networks_list_volunteer_id");
                                });

                            b1.Navigation("SocialNetworks");
                        });

                    b.Navigation("SocialNetworksList")
                        .IsRequired();

                    b.Navigation("TransferDetailsList")
                        .IsRequired();
                });

            modelBuilder.Entity("PetFamily.Domain.SpeciesContext.Entities.Breed", b =>
                {
                    b.HasOne("PetFamily.Domain.SpeciesContext.Entities.Species", null)
                        .WithMany("Breeds")
                        .HasForeignKey("species_id")
                        .HasConstraintName("fk_breeds_species_species_id");
                });

            modelBuilder.Entity("PetFamily.Domain.PetContext.Entities.Volunteer", b =>
                {
                    b.Navigation("AllOwnedPets");
                });

            modelBuilder.Entity("PetFamily.Domain.SpeciesContext.Entities.Species", b =>
                {
                    b.Navigation("Breeds");
                });
#pragma warning restore 612, 618
        }
    }
}
